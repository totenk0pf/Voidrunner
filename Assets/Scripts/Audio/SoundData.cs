using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData", order = 2)]
public class SoundData : ScriptableObject
{
    [TitleGroup("File")]
    public AudioClip audioFile;

    [TitleGroup("Settings For AudioSource")]
    public bool playStarted;
    public bool looped;
    [Space] 
    public float volume = 1;
    public float pirority = 128f;
    public float pitch = 1;
}
