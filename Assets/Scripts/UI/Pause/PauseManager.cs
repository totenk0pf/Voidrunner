using System.Collections.Generic;
using Core.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using EventType = Core.Events.EventType;

namespace UI.Pause {
    public class PauseManager : SerializedMonoBehaviour {
        private bool _isPaused;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private float transitionDuration;
        [SerializeField] private Ease transitionEase;
        [SerializeField] private List<UnityAction> onPause;
        [SerializeField] private List<UnityAction> onResume;
        
        private void Awake() {
            this.AddListener(EventType.GamePausedEvent, _ => HandlePause());
            pauseUI.SetActive(_isPaused);
        }

        private void OnDestroy() {
            this.RemoveListener(EventType.GamePausedEvent, _ => HandlePause());
            DOTween.Kill(0);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                this.FireEvent(EventType.GamePausedEvent);
            }
        }

        public void TogglePause() {
            this.FireEvent(EventType.GamePausedEvent);
        }
        
        private void HandlePause() {
            _isPaused        = !_isPaused;
            pauseUI.SetActive(_isPaused);
            pauseUI.SetActive(_isPaused);
            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            DOTween.To(() => Time.timeScale, 
                       x => {
                           Time.timeScale      = x;
                           Time.fixedDeltaTime = 0.02f * Time.timeScale;
                       },
                       _isPaused ? 0f : 1f, 
                       transitionDuration).SetId(0).SetEase(transitionEase).OnPlay(() => {
                foreach (UnityAction action in _isPaused ? onPause : onResume) {
                    action.Invoke();
                }
            });
        }
    }
}