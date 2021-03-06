// Date   : 22.02.2019 23:05
// Project: Game
// Author : bradur

using UnityEngine;
using System.Collections;

public class TeleportCaster : MonoBehaviour
{

    private PlayerConfig playerConfig;
    private GameConfig gameConfig;
    private FireConfig fireConfig;
    private PlayerHandConfig playerHandConfig;
    private FireSourceManager fireSource;

    private BezierCurve curve;
    private GameObject teleportArea;

    private FireSource teleportTarget;
    private FireSource previousTeleportTarget;

    private bool teleporting;
    // Teleport lerp parameters
    private float tpTimeStarted;
    private Vector3 tpStartPosition;

    public FireSource PreviousTeleportTarget { set { previousTeleportTarget = value; } }

    void Start()
    {
        playerConfig = ConfigManager.main.GetConfig("PlayerConfig") as PlayerConfig;
        gameConfig = ConfigManager.main.GetConfig("GameConfig") as GameConfig;
        fireConfig = ConfigManager.main.GetConfig("FireConfig") as FireConfig;
        playerHandConfig = ConfigManager.main.GetConfig("PlayerHandConfig") as PlayerHandConfig;
        InitializeCurve();
        teleportArea = Instantiate(playerConfig.TeleportAreaPrefab);
        teleportArea.transform.SetParent(transform.parent);
        //teleportArea.color = playerConfig.TeleportAreaColorAllowed;
    }

    void InitializeCurve()
    {
        if (playerConfig.DrawCurve && curve == null)
        {
            curve = Instantiate(playerConfig.CurvePrefab);
            curve.transform.SetParent(transform);
            curve.Initialize();
        }
        else if (curve != null)
        {
            if (playerConfig.DrawCurve)
            {
                curve.Enable();
            }
            else
            {
                curve.Disable();
            }
        }
    }

    void CheckTeleportationPossibilities()
    {
        RaycastHit hit;
        Vector3 endPoint = transform.TransformDirection(Vector3.forward);
        bool rayHitSomething = CastTeleportCheckRay(endPoint, out hit);
        if (rayHitSomething)
        {
            Vector3 point = hit.point;
            DrawCurve(point);
            DrawTeleportArea(point);
        }
        else
        {
            Vector3 point = transform.position + transform.forward * playerConfig.MaxDistance;
            point.y = 0f;
            DrawCurve(point);
            DrawTeleportArea(point);
        }
        DebugRayCast(hit, rayHitSomething);
    }

    void ProcessInput()
    {
        if (!teleporting && KeyManager.main.GetKeyDown(PlayerAction.Teleport))
        {
            Teleport();
        }
    }

    void Update()
    {
        if (teleporting)
        {
            Vector3 targetPosition = teleportTarget.TeleportPosition;
            float p = (Time.time - tpTimeStarted) / playerConfig.TeleportDuration;
            Vector3 lposition = Vector3.Lerp(tpStartPosition, targetPosition, p);
            lposition.y = tpStartPosition.y;
            transform.position = lposition;

            if (p >= 0.95f)
            {
                Vector3 end = teleportTarget.TeleportPosition;
                end.y = transform.position.y;
                transform.position = end;
                teleporting = false;
            }
        }
        else if (playerHandConfig.hasFire) {
            if (teleportTarget != null) {
                teleportArea.SetActive(false);
                teleportTarget = null;
            }
        }
        else
        {
            CheckTeleportationPossibilities();
        }
        ProcessInput();
    }

    bool CastTeleportCheckRay(Vector3 endPoint, out RaycastHit hit)
    {
        return Physics.Raycast(
            transform.position,
            endPoint,
            out hit,
            playerConfig.MaxDistance,
            gameConfig.GroundLayer | gameConfig.WallLayer | gameConfig.BurnableLayer
        );
    }

    void DrawTeleportArea(Vector3 endPoint)
    {
        teleportTarget = GetTeleportableFireSource(endPoint);
        Vector3 teleportAreaPosition;
        if (teleportTarget != null)
        {
            teleportAreaPosition = teleportTarget.TeleportPosition;
        }
        else
        {
            teleportAreaPosition = endPoint;
        }
        teleportAreaPosition.y = playerConfig.TeleportAreaPrefab.transform.position.y;
        teleportArea.transform.position = teleportAreaPosition;
        teleportArea.SetActive(teleportTarget != null);
    }

    void Teleport()
    {
        if (teleportTarget != null)
        {
            teleporting = true;
            tpTimeStarted = Time.time;
            tpStartPosition = transform.position;
            playerHandConfig.triggerJump = true;
            AudioManager.main.PlaySound(SoundType.Teleport);
            if (teleportTarget.IsLevelEnd)
            {
                Debug.Log("Next level!");
                LevelManager.main.LoadNextScene();
            }
        }
    }

    FireSource GetTeleportableFireSource(Vector3 endPoint)
    {
        foreach (FireSource source in FireSourceManager.main.GetLitNearSources(endPoint))
        {
            if (source == previousTeleportTarget)
            {
                continue;
            }
            Vector3 heading = source.TeleportPosition - transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, source.TeleportPosition);
            float distance = heading.magnitude;
            Vector3 direction = heading / distance; // This is now the normalized direction.
            RaycastHit hit;
            bool wasHit = Physics.Raycast(
                transform.position,
                direction,
                out hit,
                distanceToPlayer + 0.05f,
                gameConfig.WallLayer
            );
            if (gameConfig.VisualDebug)
            {
                Debug.DrawRay(
                    transform.position,
                    direction * (distanceToPlayer + 0.05f),
                    wasHit ? Color.magenta : Color.yellow
                );
            }
            if (!wasHit)
            {
                return source;
            }
        }
        return null;
    }

    void DrawCurve(Vector3 endPoint)
    {
        InitializeCurve();
        if (playerConfig.DrawCurve)
        {
            float y = 0f;
            Vector3 startPoint = new Vector3(transform.position.x, y, transform.position.z);
            endPoint.y = -1f;
            Vector3 midPoint = (endPoint + startPoint) / 2f;
            midPoint.y = playerConfig.CurveHeight;
            Vector3[] points = new Vector3[] {
                startPoint,
                //transform.TransformPoint(midPoint),
                midPoint,
                endPoint
            };
            curve.SetPoints(points);
        }
    }

    void DebugRayCast(RaycastHit hit, bool rayHitSomething)
    {
        if (gameConfig.VisualDebug)
        {
            Debug.DrawRay(
                transform.position,
                transform.TransformDirection(Vector3.forward) * (rayHitSomething ? hit.distance : playerConfig.MaxDistance),
                rayHitSomething ? playerConfig.DebugRayColorOnHit : playerConfig.DebugRayColor
            );
        }
    }
}
