using System;
using UnityEngine;

namespace Entities.Enemy.Boss {
    public class BossAttackBehavior : MonoBehaviour {
        [SerializeField] private BossAttack attack;

        [Space] 
        [SerializeField] private Transform grabPoint;
        private bool _isGrabbing;

        private void Update() {
            if (_isGrabbing) {
                attack.target.transform.position = grabPoint.position;
            }
        }

        public void OnAttack() {
            if (attack.inRange) {
                attack.DealDamage();
            }
        }

        public void OnGrab() {
            if (!attack.inRange) return;
            _isGrabbing = true;
            attack.target.GetComponent<Rigidbody>().useGravity = false;
            attack.target.GetComponent<PlayerMovementController>().enabled = false;
        }

        public void ReleaseGrab() {
            if (!_isGrabbing) return;
            _isGrabbing = false;
            attack.target.GetComponent<Rigidbody>().useGravity = true;
            attack.target.GetComponent<PlayerMovementController>().enabled = true;
            attack.DealDamage();
        }
    }
}
