using System;
using Core.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Rooms {
    public class RoomObject : MonoBehaviour {
        public bool isStartingRoom;
        [ReadOnly] public bool isInRoom;

        private void Awake() {
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            if (isStartingRoom) isInRoom = true;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            isInRoom = true;

        }
        private void OnTriggerExit(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            isInRoom = false;
        }
    }
}
