using Entities.Enemy;
using UnityEngine;

public class JuggernautHostile : EnemyState
{
    public float fastChaseSpeed;
    [SerializeField] private EnemyState _nextState;
    [SerializeField] private AnimSerializedData animData;
    
    public override EnemyState RunCurrentState() {
        if (!animator.GetBool(animData.hostileAnim.name)) TriggerAnim(animData.hostileAnim);
        if (Agent.enabled) Agent.SetDestination(target.transform.position);
  
        if (GetPathRemainingDistance(Agent) < 3f && GetPathRemainingDistance(Agent) > -1)
        {
            TriggerAnim(animData.hostileAnim);
            Agent.isStopped = true;
            return _nextState;
        }
            
        Agent.isStopped = false;
        return this;
    }
}
