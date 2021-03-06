using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum SoundType
{
    None,
    Extinguish,
    LightFire,
    Burn,
    Teleport,
    RestartSnap
}

[CreateAssetMenu(fileName = "NewAudioConfig", menuName = "New AudioConfig")]
public class AudioConfig : ScriptableObject
{

    [SerializeField]
    private AudioSource sfxPlayerPrefab;
    public AudioSource SfxPlayerPrefab { get { return sfxPlayerPrefab; } }

    public bool SfxMuted = false;

    [SerializeField]
    private AudioSource musicPlayerPrefab;
    public AudioSource MusicPlayerPrefab { get { return musicPlayerPrefab; } }

    public bool MusicMuted = false;


    [SerializeField]
    private AudioClip music;
    public AudioClip Music { get { return music; } }

    [SerializeField]
    private AudioClip endMusic;
    public AudioClip EndMusic { get { return endMusic; } }

    [SerializeField]
    [Range(0, 1f)]
    private float musicVolume = 1f;
    public float MusicVolume { get { return musicVolume; } }

    [SerializeField]
    [Range(0.1f, 3f)]
    private float musicFadeTime = 0.5f;
    public float MusicFadeTime { get { return musicFadeTime; } }

    [SerializeField]
    private List<GameSound> sounds;

    public List<GameSound> Sounds { get { return sounds; } }

}


[System.Serializable]
public class GameSound : System.Object
{

    public SoundType soundType;
    public AudioClip sound;

    public List<AudioClip> sounds;
}