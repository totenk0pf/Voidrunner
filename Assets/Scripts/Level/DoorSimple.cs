using DG.Tweening;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;

namespace Level {
    public class DoorSimple : MonoBehaviour {
        [SerializeField] private LayerMask playerLayer;
        [TitleGroup("Anim Config")] 
        [SerializeField] private GameObject doorMesh;
        public float yOffset;
        public float duration;
        public Ease easeType;
        
        private Tween _currentTween;
        
        private void OnTriggerEnter(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer)) return;
            _currentTween?.Pause();
            _currentTween = doorMesh.transform.DOLocalMoveY(yOffset, duration)
                .SetEase(easeType)
                .OnComplete(() => {
                    _currentTween = null;
                });
        }
        
        private void OnTriggerExit(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer)) return;
            _currentTween?.Pause();
            _currentTween = doorMesh.transform.DOLocalMoveY(0, duration)
                .SetEase(easeType)
                .OnComplete(() => {
                    _currentTween = null;
                });
        }
    }
}
