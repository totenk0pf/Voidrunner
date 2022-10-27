using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerHostile : EnemyState
{
    public float fastChaseSpeed;
    [SerializeField] private EnemyState _nextState;

    private Vector3 target; 
    private enum HostileStyle
    {
        Slow,
        Fast
    }

    private HostileStyle chaseType;

    public override EnemyState RunCurrentState() {
        Agent.SetDestination(target);
        Agent.isStopped = false;

        if (chaseType == HostileStyle.Slow) {
            Agent.speed = eBase.enemySpeed;
        }

        else Agent.speed = fastChaseSpeed;

        if (Agent.remainingDistance < 1.4f) {
            Agent.isStopped = true;
            return _nextState;
        }
        return this;
    }

    public override void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            chaseType = (HostileStyle)Random.Range(0, 1);
        }
    }
    
    public override void OnTriggerStay(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            target = other.transform.position;
        }
    }
}
