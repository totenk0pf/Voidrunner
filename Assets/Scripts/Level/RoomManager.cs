using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Level;
using UnityEngine;
using UnityEngine.Serialization;
using EventType = Core.Events.EventType;

// ReSharper disable InconsistentNaming

namespace Level {
    [Serializable]
    public class RoomInfo {
        public Room previousRoom;
        public Room nextRoom; 
    }
    
    public class RoomManager : MonoBehaviour {
        private List<Room> _rooms = new();

        private void Awake() {
            _rooms = FindObjectsOfType<Room>().ToList();
            foreach (Room room in _rooms) {
                if (room.type == RoomType.Hub) continue;
                room.gameObject.SetActive(false);
            }
        }

        private void Start() {
            EventDispatcher.Instance.AddListener(EventType.EnableRoom, room => EnableRoom((Room) room));
            EventDispatcher.Instance.AddListener(EventType.DisableRoom, room => DisableRoom((Room) room));
        }

        private void EnableRoom(Component roomObject) {
            roomObject.gameObject.SetActive(true);
        }   

        private void DisableRoom(Component roomObject) {
            roomObject.gameObject.SetActive(false);
        }
    }
}
