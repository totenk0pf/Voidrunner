using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerAttackBehavior : MonoBehaviour {
    [SerializeField] private WalkerAttack _walkerAttack;

    public void OnAttack() {
        if (_walkerAttack.inRange) {
            _walkerAttack.DealDamage();
        }
    }

    public void ResetAttack() {
        _walkerAttack.RestartAttack();
    }
}
