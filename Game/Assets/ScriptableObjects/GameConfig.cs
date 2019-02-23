// Date   : 29.12.2018 12:52
// Project: VillageCaveGame
// Author : bradur

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "NewGameConfig", menuName = "New GameConfig")]
public class GameConfig : ScriptableObject
{

    [Header("World")]
    [SerializeField]
    private LayerMask groundLayer;
    public LayerMask GroundLayer { get { return groundLayer; } }

    [SerializeField]
    private LayerMask wallLayer;
    public LayerMask WallLayer { get { return wallLayer; } }

    [SerializeField]
    private LayerMask torchLayer;
    public LayerMask TorchLayer { get { return torchLayer; } }

    [SerializeField]
    private LayerMask burnableLayer;
    public LayerMask BurnableLayer { get { return burnableLayer; } }


    [Header("Debugging")]

    public bool VisualDebug = false;

    public LogLevel LogLevel;

}
