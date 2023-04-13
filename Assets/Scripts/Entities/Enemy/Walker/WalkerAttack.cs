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
            StartCoroutine(StartAttack());
        }

        return this;
    }

    private IEnumerator StartAttack() {
        if (_canSwitchState) yield break;
        _canAttack = false;
        _isAttacking = true;
        if (_moveWithRootMotion.canMove) _moveWithRootMotion.canMove = false;
        TriggerAnim(_animData.attackAnim[0]);
        yield return StartCoroutine(FinishAnimation());
        yield return StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack() {
        yield return new WaitForSeconds(attackDelay);
        StartCoroutine(StartAttack());
    }

    private IEnumerator FinishAnimation() {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        _isAttacking = false;
    }
    
    public override void OnTriggerExit(Collider other) {
        if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
            StopAllCoroutines();
            if (_isAttacking) {
                StartCoroutine(FinishAnimation());
            }
            
            foreach (var anim in _animData.attackAnim) {
                ResetAnim(anim);
            }
            
            _canAttack = false;
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
