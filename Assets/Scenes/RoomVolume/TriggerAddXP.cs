using System;
using System.Collections;
using Core.Events;
using StaticClass;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Scenes.RoomVolume {
    public class TriggerAddXP : MonoBehaviour {
        private bool _isYourFridgeRunning;

        private void OnTriggerStay(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !_isYourFridgeRunning) {
                StartCoroutine(AddXpRoutine());
            }
        }

        private IEnumerator AddXpRoutine() {
            _isYourFridgeRunning = true;
            EventDispatcher.Instance.FireEvent(EventType.AddXP, 20f);
            yield return new WaitForSeconds(3.5f);
            _isYourFridgeRunning = false;
        }
    }
}
