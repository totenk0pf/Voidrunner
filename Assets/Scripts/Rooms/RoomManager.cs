using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Patterns;
using UnityEngine;
using EventType = Core.Events.EventType;

// ReSharper disable InconsistentNaming

namespace Rooms {
    [Serializable]
    public class RoomInfo {
        public RoomObject Room1;
        public RoomObject Room2; 
    }
    
    public class RoomManager : MonoBehaviour {
        private List<RoomObject> _rooms = new();

        private void Awake() {
            _rooms = FindObjectsOfType<RoomObject>().ToList();
            foreach (var room in _rooms) {
                if (room.isStartingRoom) return;
                room.gameObject.SetActive(false);
            }
        }

        private void Start() {
            EventDispatcher.Instance.AddListener(EventType.EnableRoom, room => EnableRoom((RoomObject) room));
            EventDispatcher.Instance.AddListener(EventType.DisableRoom, room => DisableRoom((RoomObject) room));
        }

        private void EnableRoom(Component roomObject) {
            roomObject.gameObject.SetActive(true);
        }   

        private void DisableRoom(Component roomObject) {
            roomObject.gameObject.SetActive(false);
        }
    }
}
