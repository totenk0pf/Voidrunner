using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using StaticClass;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Level {
    public class EndLevelTrigger : SerializedMonoBehaviour {
        [SerializeField] private GameObject endingCanvas;
        [SerializeField] private TextMeshProUGUI scrollText;
        [SerializeField] private float scrollDuration;
        [SerializeField] private float scrollDelay;
        [SerializeField] private List<UnityAction> onEnterAction;
        [SerializeField] private List<UnityAction> onFinishScroll;
        [SerializeField] private LayerMask playerMask;
        private float _textHeight;
        private float _screenYExtent;
        private float _targetYPos;

        private void Awake() {
            _textHeight    = scrollText.rectTransform.rect.height;
            _screenYExtent = Screen.height * 0.5f;
            _targetYPos    = _textHeight + _screenYExtent;
        }

        public void EnableEndingCanvas() {
            endingCanvas.SetActive(true);
        }
        
        public void TriggerTextScroll() {
            DOTween.To(() => scrollText.rectTransform.position.y,
                       x => scrollText.rectTransform.position
                           = new Vector3(scrollText.rectTransform.position.x, x, scrollText.rectTransform.position.z),
                       _targetYPos, 
                       scrollDuration).SetDelay(scrollDelay).OnComplete(() => {
                foreach (UnityAction action in onFinishScroll) {
                    action();
                }
            });
        }

        public void ReturnToMenu() {
            StartCoroutine(LoadMenuCoroutine());
        }

        private IEnumerator LoadMenuCoroutine() {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(0);
            yield return null;
        }

        private void OnTriggerEnter(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) return;
            foreach (UnityAction action in onEnterAction) {
                action();
            }
        }
    }
}