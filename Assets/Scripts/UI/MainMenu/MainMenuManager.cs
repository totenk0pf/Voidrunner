using System;
using Core.Logging;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace UI.MainMenu {
    public class MainMenuManager : MonoBehaviour {
        private bool _settingsEnabled;
        private bool _exitEnabled;
        [SerializeField] private MainMenuAudioHelper _audioHelper;
        [SerializeField] private GameObject settingsUI;
        [SerializeField] private GameObject exitUI;
        [SerializeField] private DOTweenAnimation settingsAnim;
        [SerializeField] private DOTweenAnimation exitAnim;
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
            _exitEnabled     = !_exitEnabled;
            tooltip.disabled = !tooltip.disabled;
        }

        public void ToggleExit() {
            if (_exitEnabled) {
                exitAnim.DOPlayBackwards();
            } else {
                exitUI.SetActive(true);
                exitAnim.DORestart();
            }
        }

        public void PlayPanelOpenSound() {
            _audioHelper.PlayButtonSound(MainMenuAudioHelper.UISelection.PanelOpen);
        }

        public void PlayButtonDenySound() {
            _audioHelper.PlayButtonSound(MainMenuAudioHelper.UISelection.ButtonDeny);
        }
    }
}