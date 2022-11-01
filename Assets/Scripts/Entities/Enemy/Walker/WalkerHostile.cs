using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerHostile : EnemyState
{
    public float fastChaseSpeed;
    [SerializeField] private EnemyState _nextState;
    private enum HostileStyle
    {
        Slow,
        Fast
    }

    private HostileStyle chaseType;

    public override EnemyState RunCurrentState() {
        Agent.SetDestination(target.transform.position);
        Agent.isStopped = false;

        if (chaseType == HostileStyle.Slow) {
            Agent.speed = enemyBase.enemySpeed;
        }

        else Agent.speed = fastChaseSpeed;

        if (Agent.remainingDistance < 1.4f) {
            Agent.isStopped = true;
            return _nextState;
        }
        return this;
    }
}
