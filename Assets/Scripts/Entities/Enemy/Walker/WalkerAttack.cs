using System.Collections;
using Entities.Enemy;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;

public class WalkerAttack : EnemyState
{
    public float attackDelay;
    [HideInInspector] public bool inRange; 

    [Title("Data")]
    [SerializeField] private WalkerHostile _previousState;
    [SerializeField] private AnimSerializedData _animData;

    [Title("Refs")] 
    [SerializeField] private EnemyMoveRootMotion _moveWithRootMotion;
    
    private bool _isAttacking = false;
    private bool _canSwitchState = false;
    private bool _canAttack = true;
    
    public override EnemyState RunCurrentState() {

        if (_canSwitchState && !_isAttacking) {
            return _previousState;
        }

        if (_canAttack) {
            StartAttack();
        }

        return this;
    }

    private void StartAttack() {
        _canAttack = false;
        _isAttacking = true;
        if (_moveWithRootMotion.canMove) _moveWithRootMotion.canMove = false;
        TriggerAnim(_animData.attackAnim[0]);
    }

    public void RestartAttack() {
        _isAttacking = false;
        if (_canSwitchState) return;
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack() {
        yield return new WaitForSeconds(attackDelay);
        StartAttack();
    }
    
    
    public override void OnTriggerExit(Collider other) {
        if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
            StopAllCoroutines();

            
            foreach (var anim in _animData.attackAnim) {
                ResetAnim(anim);
            }
            
            _canAttack = false;
            _isAttacking = false;
            inRange = false;
            
            _canSwitchState = true;
            
            _moveWithRootMotion.canMove = true;
            _previousState.ResetState();
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
