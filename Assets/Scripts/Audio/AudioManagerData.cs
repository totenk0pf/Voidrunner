using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioManagerData", menuName = "Audio/AudioManagerData", order = 1)]
public class AudioManagerData : ScriptableObject
{
    [ShowInInspector] public static List<SoundData> soundData = new();
}
