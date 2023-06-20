using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat {
    [RequireComponent(typeof(TrailRenderer))]
    public class WeaponTrail : MonoBehaviour {
        private TrailRenderer _renderer;

        private void Awake() {
            _renderer          = GetComponent<TrailRenderer>();
            _renderer.emitting = false;
            this.AddListener(EventType.AttackBeginEvent, _ => OnTrailUpdate(true));
            this.AddListener(EventType.AttackEndEvent, _ => OnTrailUpdate(false));
        }

        private void OnTrailUpdate(bool state) {
            _renderer.emitting = state;
        }
    }
}