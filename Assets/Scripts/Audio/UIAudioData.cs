using System;
using UnityEngine;

namespace Audio {
    [Serializable]
    public class UIAudio {
        public AudioClip buttonConfirmSound;
        public AudioClip buttonDenySound;
        public AudioClip panelOpenSound;
        public AudioClip panelCloseSound;
    }
    
    [CreateAssetMenu(fileName = "UIAudio", menuName = "Audio/UIAudio", order = 2)]
    public class UIAudioData : ScriptableObject {
        public UIAudio uiAudio;
    }
}