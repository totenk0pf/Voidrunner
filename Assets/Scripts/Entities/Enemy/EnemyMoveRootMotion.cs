using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Enemy {
    public class EnemyMoveRootMotion : MonoBehaviour {
        private Animator _animator;
        public bool canMove = true; 

        private void Awake() {
            _animator = this.gameObject.GetComponent<Animator>();
        }

        private void OnAnimatorMove() {
            if (_animator && canMove) {
                transform.root.position += _animator.deltaPosition;
            }
        }
    }
}