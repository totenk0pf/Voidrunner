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

        public void PlayButtonSound(int type) {
            var clip = type switch {
                0 => uiAudioData.uiAudio.buttonConfirmSound,
                1 => uiAudioData.uiAudio.buttonDenySound,
                2 => uiAudioData.uiAudio.panelOpenSound,
                3 => uiAudioData.uiAudio.panelCloseSound,
            };
            
            AudioManager.Instance.PlayClip(clip);
        }
        
        public void PlayButtonSound(UISelection type) {
            var clip = type switch {
                UISelection.ButtonConfirm => uiAudioData.uiAudio.buttonConfirmSound,
                UISelection.ButtonDeny => uiAudioData.uiAudio.buttonDenySound,
                UISelection.PanelClose => uiAudioData.uiAudio.panelOpenSound,
                UISelection.PanelOpen => uiAudioData.uiAudio.panelCloseSound,
            };
            
            AudioManager.Instance.PlayClip(clip);
        }
    }
}
