using System.Collections;
using System.Collections.Generic;
using Core.Collections;
using Entities.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;

public class WalkerHostile : EnemyState
{
    [Title("Data")]
    [SerializeField] private WalkerAttack _nextState;
    [SerializeField] private AnimSerializedData _animData;
    
    private bool _canSwitchChaseState = true;
    private bool _canSwitchState = true;
    
    private AnimParam _currAnim; 
    
    public override EnemyState RunCurrentState() {
        transform.root.LookAt(target.transform.position);
        if (_canSwitchChaseState) {
            StartCoroutine(SwitchChaseState());
        }
        
        if (_nextState.inRange && _canSwitchState) {
            _canSwitchState = false;
            TriggerAnim(_currAnim);
            return _nextState;
        }
        
        return this;
    }

    public void ResetState() {
        _canSwitchState = true;
        _canSwitchChaseState = true;
    }

    private IEnumerator SwitchChaseState() {
        _canSwitchChaseState = false;
        
        //reset anims
        foreach (var animParam in _animData.hostileAnim) {
            animator.SetBool(animParam.name, false);
        }

        _currAnim = _animData.hostileAnim[Random.Range(0, _animData.hostileAnim.Count - 1)];
        TriggerAnim(_currAnim);
        yield return null;
    }
}
