using System;
using Core.Logging;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace UI.MainMenu {
    public class MainMenuManager : MonoBehaviour {
        private bool _settingsEnabled;
        [TitleGroup("Settings")] [SerializeField] private GameObject settingsUI;
        [TitleGroup("Settings")] [SerializeField] private DOTweenAnimation settingsAnim;

        private bool _exitEnabled;
        [TitleGroup("Exit")] [SerializeField] private GameObject exitUI;
        [TitleGroup("Exit")] [SerializeField] private DOTweenAnimation exitAnim;
        
        [TitleGroup("Tooltip")]
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
    }
}