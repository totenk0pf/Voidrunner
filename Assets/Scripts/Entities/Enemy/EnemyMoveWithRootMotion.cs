using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveWithRootMotion : MonoBehaviour {
    private Animator _animator;

    private void Awake() {
        _animator = this.gameObject.GetComponent<Animator>();
    }

    private void OnAnimatorMove() {
        if (_animator) {
            transform.root.position += _animator.deltaPosition;
        }
    }
}
