using System;
using System.Collections;
using Audio;
using Core.Events;
using DG.Tweening;
using Level;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;
using UnityEngine.Serialization;
using EventType = Core.Events.EventType;
using Random = UnityEngine.Random;

namespace Level {
    [RequireComponent(typeof(BoxCollider))]
    public class Door : MonoBehaviour {
        protected enum AudioType {
            Open,
            Close,
            CloseImpact
        }
        
        public GameObject doorContainer;
        [SerializeField] private GameObject lockContainer;
        
        [TitleGroup("Room Config")] 
        public bool isBacktrackDisabled; //set if door open on previous room
        [SerializeField] protected LayerMask playerLayer;
        public RoomInfo roomInfo = new();
        
        [TitleGroup("Anim Config")] 
        [SerializeField] private GameObject doorMesh;
        public float yOffset;
        public float duration;
        public Ease easeType;

        [TitleGroup("Audio Config")] 
        public DoorAudioData audioData;

        [TitleGroup("Checkpoint Config")] 
        public bool isCheckpoint;
        private bool _hasCheckPointSet;

        [Space] 
        [ReadOnly] public bool canOpen;
        
        private Tween _currentTween;
        private bool _canRunTween = true;
        
        private Room _currentRoom;
        private Room _nextRoom;
        
        protected void Start() {
            _currentRoom = roomInfo.previousRoom;
            _nextRoom    = roomInfo.nextRoom;
            canOpen      = true;

            this.AddListener(EventType.DoorInvoked, cb => CheckDoor());
            this.AddListener(EventType.RoomLock, room => UpdateLock((Room) room, true));
            this.AddListener(EventType.RoomUnlock, room => UpdateLock((Room) room, false));
            CheckDoor();
        }

        public void ResetDoor(Room exception = null) {
            _canRunTween = false;
            _currentTween?.Kill();
            
            _currentRoom = roomInfo.previousRoom;
            _nextRoom = roomInfo.nextRoom;
            
            var t = doorMesh.transform.localPosition;
            doorMesh.transform.localPosition = new Vector3(t.x, 0, t.z);
            _currentRoom.gameObject.SetActive(false);

            if (_currentRoom == exception) {
                _currentRoom.gameObject.SetActive(true);
            }
            else {
                _nextRoom.gameObject.SetActive(false);
                doorContainer.SetActive(false);   
            }

            StartCoroutine(Delay());
        }

        private IEnumerator Delay() {
            yield return new WaitForSeconds(1.2f);
            _canRunTween = true;
        }
        

        private void CheckDoor() {
            if (!_currentRoom.gameObject.activeInHierarchy && !_nextRoom.gameObject.activeInHierarchy) {
                doorContainer.SetActive(false);
            } else {
                if (!doorContainer.activeInHierarchy) doorContainer.SetActive(true);
            }
        }

        private void UpdateLock(Room room, bool state) {
            if (room == _currentRoom || room == _nextRoom) lockContainer.SetActive(state);
        }

        public void ResetLock() {
            lockContainer.SetActive(false);
        }

        protected void OnTriggerEnter(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer) && _canRunTween) return;
            if (!_canRunTween) return;
            if (_nextRoom.gameObject.activeInHierarchy && isBacktrackDisabled) return;
            if (!canOpen) return;
            if (!InheritedCheck(other)) return;
            if (_currentRoom.enemiesInRoom.Count > 0) return;
            
            this.FireEvent(EventType.EnableRoom, _nextRoom);
            this.FireEvent(EventType.OnPlayerEnterDoor, this);
            this.FireEvent(EventType.DoorInvoked);
            PlayAudio(AudioType.Open);

            if (isCheckpoint && !_hasCheckPointSet) {
                _hasCheckPointSet = false;
                this.FireEvent(EventType.SetCheckpoint, _currentRoom);
            }
            
            _currentTween?.Pause();
            _currentTween = doorMesh.transform.DOLocalMoveY(yOffset, duration)
                .SetEase(easeType)
                .OnComplete(() => {
                    _currentTween = null;
                });
        }

        protected virtual bool InheritedCheck(Collider other) {
            return true;
        }

        protected void OnTriggerExit(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer)) return;
            if (!_canRunTween) return;
            PlayAudio(AudioType.Close);
            _currentTween?.Pause();
            _currentTween = doorMesh.transform.DOLocalMoveY(0, duration)
                .SetEase(easeType)
                .OnComplete(() => {
                    PlayAudio(AudioType.CloseImpact);
                    if (_currentRoom.IsInRoom) {
                        this.FireEvent(EventType.DisableRoom, _nextRoom);
                        this.FireEvent(EventType.DoorInvoked);
                        this.FireEvent(EventType.RoomEntered, _currentRoom);
                    } else {
                        this.FireEvent(EventType.DisableRoom, _currentRoom);
                        this.FireEvent(EventType.RoomEntered, _nextRoom);
                        this.FireEvent(EventType.DoorInvoked);
                        
                        if (isBacktrackDisabled) return;
                        if (_currentRoom == roomInfo.previousRoom) {
                            _currentRoom = roomInfo.nextRoom;
                            _nextRoom = roomInfo.previousRoom;
                        } else {
                            _currentRoom = roomInfo.previousRoom;
                            _nextRoom = roomInfo.nextRoom;
                        }
                    }
                    _currentTween = null;
                });
        }

        protected void PlayAudio(AudioType type) {
            var list = type switch {
                AudioType.Close => audioData.doorAudio.closeAudios,
                AudioType.CloseImpact => audioData.doorAudio.closeImpactAudios,
                _=> audioData.doorAudio.openAudios
            };
            
            AudioManager.Instance.PlayClip( transform.position, list[Random.Range(0, list.Count)]);
        }
    }
}