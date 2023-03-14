using System.Collections;
using System.Collections.Generic;
using StaticClass;
using UnityEngine;

public class WalkerAttack : EnemyState
{
    public float attackDelay;
    
    [SerializeField] private WalkerHostile _previousState;
    [SerializeField] private EnemyState _nextState;

    public bool inRange;

    private bool _isAttacking = false;
    private bool _canSwitchState = false;
    private bool _canAttack = true;

    public override EnemyState RunCurrentState() {

        if (_canSwitchState && !_isAttacking) {
            return _previousState;
        }

        if (_canAttack) {
            StartCoroutine(StartAttack());
        }

        return this;
    }

    IEnumerator StartAttack() {
        _canAttack = false;
        _isAttacking = true;

        DealDamage();

        //Anim Time Here
        yield return new WaitForSeconds(0.2f);
        _isAttacking = false;
        yield return DelayAttack();
    }

    IEnumerator DelayAttack() {
        yield return new WaitForSeconds(attackDelay);
        yield return StartCoroutine(StartAttack());
    }
    
    public override void OnTriggerExit(Collider other) {
        if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
            StopAllCoroutines();

            _previousState.canSwitchChaseType = true;
            Agent.ResetPath();
            _canAttack = false;
            inRange = false;
            
            _canSwitchState = true;
        }
    }

    public override void OnTriggerEnter(Collider other) {
        if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
            target = other.gameObject;
            inRange = true;
            _canAttack = true;
           
            _canSwitchState = false;
        }
    }
}
