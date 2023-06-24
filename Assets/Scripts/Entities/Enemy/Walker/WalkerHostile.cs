using System.Collections;
using System.Collections.Generic;
using Audio;
using Core.Collections;
using DG.Tweening;
using Entities.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class WalkerHostile : EnemyState
{
    [Title("Data")]
    [SerializeField] private WalkerAttack _nextState;
    [SerializeField] private AnimSerializedData _animData;
    
    [SerializeField] private EnemyMovelistData _hostileAnimData;

    private bool _canSwitchChaseState = true;
    private bool _canSwitchState = true;
    
    private AnimParam _currHostileAnim;
    private bool _canSetAnim;

    public override EnemyState RunCurrentState() {
        if (NavMesh.SamplePosition(target.transform.position, out var hit, Agent.height / 2, NavMesh.AllAreas)) {
            Agent.SetDestination(target.transform.position);
            _canSetAnim = true;

            if (_currHostileAnim.name == null) {
                StartCoroutine(SwitchChaseState());
            } 
        }
        
        else {
            if (_canSetAnim) {
                _canSetAnim = false;
                ResetAnim(_currHostileAnim);
                _currHostileAnim.name = null;
            };
        }
        
        if (_nextState.inRange && _canSwitchState) {
            _canSwitchState = false;
            ResetAnim(_currHostileAnim);
            _currHostileAnim.name = null;
            return _nextState;
        }
        
        return this;
    }

    protected override void RestartState() {
        ResetState();
    }

    public void ResetState() {
        _canSwitchState = true;
        _canSwitchChaseState = true;
    }

    private IEnumerator SwitchChaseState() {
        _canSwitchChaseState = false;

        //reset anims
        foreach (var animParam in _animData.hostileAnim) {
            ResetAnim(animParam);
        }

        _currHostileAnim = GetItemFromMoveList(_hostileAnimData.moveList);
        TriggerAnim(_currHostileAnim);
        yield return null;
    }
}
