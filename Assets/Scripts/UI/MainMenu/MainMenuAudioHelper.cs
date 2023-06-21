using System;
using Audio;
using UnityEngine;

namespace UI.MainMenu {
    public class MainMenuAudioHelper : MonoBehaviour {
        [Serializable]
        public enum UISelection {
            ButtonDeny,
            ButtonConfirm,
            PanelClose,
            PanelOpen,
        }
        
        public UIAudioData uiAudioData;
        [SerializeField] private float volume = 1f;
        
        public void PlayButtonSound(int type) {
            AudioClip clip = type switch {
                0 => uiAudioData.uiAudio.buttonConfirmSound,
                1 => uiAudioData.uiAudio.buttonDenySound,
                2 => uiAudioData.uiAudio.panelOpenSound,
                3 => uiAudioData.uiAudio.panelCloseSound,
            };
            
            AudioManager.Instance.PlayClip(clip, volume);
        }
        
        public void PlayButtonSound(UISelection type) {
            AudioClip clip = type switch {
                UISelection.ButtonConfirm => uiAudioData.uiAudio.buttonConfirmSound,
                UISelection.ButtonDeny => uiAudioData.uiAudio.buttonDenySound,
                UISelection.PanelClose => uiAudioData.uiAudio.panelOpenSound,
                UISelection.PanelOpen => uiAudioData.uiAudio.panelCloseSound,
            };
            
            AudioManager.Instance.PlayClip(clip, volume);
        }
    }
}
