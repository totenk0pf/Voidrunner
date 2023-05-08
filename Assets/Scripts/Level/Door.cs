using System;
using Core.Events;
using DG.Tweening;
using Rooms;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;
using UnityEngine.Serialization;
using EventType = Core.Events.EventType;

namespace Level {
    [RequireComponent(typeof(BoxCollider))]
    public class Door : MonoBehaviour {
        public GameObject doorContainer;
        
        [FormerlySerializedAs("disableBacktrack")] [FormerlySerializedAs("canBacktrack")] [TitleGroup("Room Config")] 
        public bool isBacktrackDisabled; //set if door open on previous room
        [SerializeField] private LayerMask playerLayer;
        [Space] 
        public RoomInfo roomInfo = new();
        
        [TitleGroup("Anim Config")] 
        [SerializeField] private GameObject doorMesh;
        public float yOffset;
        public float duration;
        public Ease easeType;

        [Space] 
        [ReadOnly] public bool canOpen = true;
        
        private Tween _currentTween;
        
        private RoomObject _currentRoom;
        private RoomObject _nextRoom;
        

        private void Start() {
            _currentRoom = roomInfo.Room1;
            _nextRoom = roomInfo.Room2;
            
            EventDispatcher.Instance.AddListener(EventType.DoorInvoked, cb => CheckDoor());
            CheckDoor();
        }

        private void CheckDoor() {
            if (!_currentRoom.gameObject.activeInHierarchy && !_nextRoom.gameObject.activeInHierarchy) {
                doorContainer.SetActive(false);
            }

            else {
                if (!doorContainer.activeInHierarchy) doorContainer.SetActive(true);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer)) return;
            if (_nextRoom.gameObject.activeInHierarchy && isBacktrackDisabled) return;
            if (!canOpen) return;
            
            EventDispatcher.Instance.FireEvent(EventType.EnableRoom, _nextRoom);
            EventDispatcher.Instance.FireEvent(EventType.DoorInvoked);
            
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

                    if (_currentRoom.isInRoom) {
                        EventDispatcher.Instance.FireEvent(EventType.DisableRoom, _nextRoom);
                        EventDispatcher.Instance.FireEvent(EventType.DoorInvoked);
                    }
                    else {
                        EventDispatcher.Instance.FireEvent(EventType.DisableRoom, _currentRoom);
                        EventDispatcher.Instance.FireEvent(EventType.DoorInvoked);
                        
                        if (isBacktrackDisabled) return;
                        if (_currentRoom == roomInfo.Room1) {
                            _currentRoom = roomInfo.Room2;
                            _nextRoom = roomInfo.Room1;
                        }
                        else {
                            _currentRoom = roomInfo.Room1;
                            _nextRoom = roomInfo.Room2;
                        }
                    }
                });
        }
    }
}