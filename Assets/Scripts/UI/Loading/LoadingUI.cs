using Core;
using UnityEngine;
using Core.Events;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace UI.Loading {
    public class LoadingUI : MonoBehaviour {
        [SerializeField] private Slider loadingBar;
        
        private void Awake() {
            this.AddListener(EventType.LoadingProgressEvent, progress => loadingBar.value = (float) progress);
        }

        public void StartGame() {
            GameManager.Instance.StartGame();
        }

        public void QuitToMenu() {
            GameManager.Instance.QuitToMenu();
        }
    }
}