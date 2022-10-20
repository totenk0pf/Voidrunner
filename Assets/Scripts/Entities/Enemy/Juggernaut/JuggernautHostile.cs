using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautHostile : EnemyState
{
    public float fastChaseSpeed;
    [SerializeField] private EnemyState _nextState;

    private Vector3 target;
    public override EnemyState RunCurrentState() {
        Agent.SetDestination(target);
        Agent.isStopped = false;
        Agent.speed = fastChaseSpeed;

        if (Vector3.Distance(transform.position, target) < 2f) {
            Agent.isStopped = true;
            return _nextState;
        }
        return this;
    }

    public override void OnTriggerEnter(Collider other) {
    }

    public override void OnTriggerStay(Collider other) {
        if (other.tag == "Player") {
            target = other.transform.position;
        }
    }
}
