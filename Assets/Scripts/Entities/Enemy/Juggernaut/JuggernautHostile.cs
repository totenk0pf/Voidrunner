using Entities.Enemy;
using Entities.Enemy.Juggernaut;
using UnityEngine;

public class JuggernautHostile : EnemyState
{
    public float fastChaseSpeed;
    [SerializeField] private JuggernautAttack _nextState;
    [SerializeField] private AnimSerializedData animData;
    
    public override EnemyState RunCurrentState() {
        if (!animator.GetBool(animData.hostileAnim.name)) TriggerAnim(animData.hostileAnim);
        if (Agent.enabled) Agent.SetDestination(target.transform.position);
  
        if (_nextState.inRange) {
            Agent.isStopped = true;
            return _nextState;
        }
        
        return this;
    }
}
