using System;
using Core.Logging;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace UI.MainMenu {
    public class MainMenuManager : MonoBehaviour {
        private bool _settingsEnabled;
        [SerializeField] private MainMenuAudioHelper _audioHelper;
        [SerializeField] private GameObject settingsUI;
        [SerializeField] private DOTweenAnimation settingsAnim;
        [SerializeField] private MainMenuTooltip tooltip;

        public void ToggleSettings() {
            if (_settingsEnabled) {
                settingsAnim.DOPlayBackwards();
            } else {
                settingsUI.SetActive(true);
                settingsAnim.DORestart();
            }
        }

        public void ToggleStates() {
            _settingsEnabled = !_settingsEnabled;
            tooltip.disabled = !tooltip.disabled;
        }

        public void PlayPanelOpenSound() {
            _audioHelper.PlayButtonSound(MainMenuAudioHelper.UISelection.PanelOpen);
        }

        public void PlayButtonDenySound() {
            _audioHelper.PlayButtonSound(MainMenuAudioHelper.UISelection.ButtonDeny);
        }
    }
}