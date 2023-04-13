using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Entities.Enemy.Boss {
    public class GrabBehavior : MonoBehaviour {
        [SerializeField] private GrabHitbox _grabHitbox;
        [SerializeField] private Transform _grabPoint;
        [Space] [SerializeField] private EnemyState _attackState;

        private PlayerMovementController _player;
        private Rigidbody _pRb;

        private void Update() {
            if (_grabHitbox.grabbed && _player) {
                _player.transform.position = _grabPoint.position;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (_grabHitbox.grabbed) {
                if (!_player) _player = other.gameObject.GetComponent<PlayerMovementController>();
                if (!_pRb) _pRb = other.gameObject.GetComponent<Rigidbody>();
                StartCoroutine(GrabAttack());
            }
        }
        
        private IEnumerator GrabAttack() {
            if (!_player || !_pRb) yield return null;
            
            _player.enabled = false;
            _pRb.isKinematic = true;

            yield return new WaitForSeconds(3.775f);
            _grabHitbox.grabbed = false;

            _player.enabled = true;
            _pRb.isKinematic = false;
            
            _attackState.DealDamage();
        }
    


        // private PlayerMovementController _player;
        // private Rigidbody _rb;
        //
        // private bool isGrabbing;
        // private bool canGrab;
        //
        // private void Awake() {
        //     StartCoroutine(GrabAttack());
        // }
        //
        // private void OnTriggerEnter(Collider other) {
        //     if (_grabHitbox.grabbed) {
        //         if (!_player) _player = other.GetComponent<PlayerMovementController>();
        //         if (!_rb) _rb = other.GetComponent<Rigidbody>();
        //         if (canGrab) isGrabbing = true;
        //     }
        // }
        //
        // private void OnTriggerExit(Collider other) {
        //     if (_player) _player = null;
        //     if (_rb) _rb = null;
        // }
        //
        // private IEnumerator GrabAttack() {
        //     while (true) {
        //         if (!_player || !_rb) {
        //             isGrabbing = false;
        //             yield return null;
        //         }
        //         if (isGrabbing) {
        //             canGrab = false;
        //
        //             _grabHitbox.grabbed = false;
        //
        //             _player.enabled = false;
        //             _rb.isKinematic = true;
        //
        //             _player.transform.position = _grabPoint.position;
        //
        //             // yield return new WaitForSeconds(3f);
        //
        //             _player.enabled = true;
        //             _rb.isKinematic = false;
        //             
        //             // _attackState.DealDamage();
        //             isGrabbing = false;
        //             canGrab = true;
        //         }
        //         yield return null;
        //     }
        // }
    }
}