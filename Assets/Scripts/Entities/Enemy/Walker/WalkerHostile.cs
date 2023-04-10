using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerHostile : EnemyState
{
    public float fastChaseSpeed;
    [SerializeField] private WalkerAttack _nextState;
    private enum HostileStyle
    {
        Slow,
        Fast
    }

    private HostileStyle chaseType;
    public bool canSwitchChaseType = true;

    public override EnemyState RunCurrentState() {
        Agent.SetDestination(target.transform.position);
        Agent.isStopped = false;

        if (canSwitchChaseType) {
            StartCoroutine(ChangeChaseType());
        }

        if (_nextState.inRange) {
            Agent.isStopped = true;
            return _nextState;
        }
        
        return this;
    }

    IEnumerator ChangeChaseType() {
        canSwitchChaseType = false;

        if (Random.Range(0, 1) == 0) {
            Agent.speed = enemyBase.enemySpeed;
        }
        else Agent.speed = fastChaseSpeed;

        yield return null;
    }
}
