using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using StaticClass;
using Unity.VisualScripting;
using UnityEngine;

namespace Level {
    public class Elevator : MonoBehaviour {
        [SerializeField] private LayerMask playerLayer;

        [TitleGroup("Elevator Door Mesh")] 
        public GameObject door1;
        public GameObject door2;

        [TitleGroup("Elevator Position")] 
        public Transform pointA;
        public Transform pointB;
        
        [TitleGroup("Anim Config")]
        public float doorCloseDuration;
        public Ease doorEaseType;
        
        [Space]
        public float elevatorDuration;
        public Ease elevatorEaseType;

        [TitleGroup("Audio Config")] 
        public AudioClip elevatorDoorSound;
        public AudioClip elevatorLoopSound;

        private Vector3 _originalDoor1Pos;
        private Vector3 _originalDoor2Pos;

        private Transform _currentPoint;

        private void Start() {
            _currentPoint = pointA;
            _originalDoor1Pos = door1.transform.localPosition;
            _originalDoor2Pos = door2.transform.localPosition;
        }

        private void OnTriggerEnter(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer)) return;
            other.gameObject.transform.SetParent(gameObject.transform);
            var consistentY = other.gameObject.transform.localPosition.y;
            other.gameObject.GetComponent<PlayerMovementController>().canGravity = false;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;

            var dest = _currentPoint == pointA ? pointB : pointA;
            _currentPoint = _currentPoint == pointA ? pointB : pointA;
            
            var s = DOTween.Sequence();
            s
                .Append(door1.transform.DOLocalMoveX(-2, doorCloseDuration).SetEase(doorEaseType))
                .Insert(0, door2.transform.DOLocalMoveX(-2.875f, doorCloseDuration).SetEase(doorEaseType))
                .Append(transform.DOMoveY(dest.position.y, elevatorDuration).SetEase(elevatorEaseType))
                .Append(DOVirtual.DelayedCall(doorCloseDuration, () => {
                    door1.transform.DOLocalMoveX(_originalDoor1Pos.x, doorCloseDuration).SetEase(doorEaseType);
                    door2.transform.DOLocalMoveX(_originalDoor2Pos.x, doorCloseDuration).SetEase(doorEaseType);
                }))
                .OnUpdate(() => {
                    other.transform.localPosition
                        = new Vector3(other.transform.localPosition.x, consistentY, other.transform.localPosition.z);
                });
        }
        
        private void OnTriggerExit(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer)) return;
            other.gameObject.GetComponent<PlayerMovementController>().canGravity = true;
            other.gameObject.GetComponent<Rigidbody>().useGravity = true;
            other.gameObject.transform.SetParent(null);
        }
    }
}
