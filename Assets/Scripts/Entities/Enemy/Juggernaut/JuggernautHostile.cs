using UnityEngine;

public class JuggernautHostile : EnemyState
{
    public float fastChaseSpeed;
    [SerializeField] private EnemyState _nextState;
    
    public override EnemyState RunCurrentState() {
        Agent.SetDestination(target.transform.position);
        Agent.isStopped = false;
        Agent.speed = fastChaseSpeed;

        if (Agent.remainingDistance < 3f) {
            Agent.isStopped = true;
            return _nextState;
        }
        return this;
    }
}
