using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio {
    [Serializable]
    public class DoorAudio {
        [Header("Door Open")] 
        public List<AudioClip> openAudios;
        
        [Header("Door Close")] 
        public List<AudioClip> closeAudios;
        public List<AudioClip> closeImpactAudios;
    }
    
    [CreateAssetMenu(fileName = "DoorAudio", menuName = "Audio/DoorAudio", order = 3)]
    public class DoorAudioData : ScriptableObject {
        public DoorAudio doorAudio;
    }
}