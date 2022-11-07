using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Enemy.Boss {
    public class BossHostile : EnemyState
    {
        [SerializeField] private EnemyState nextState;
        [SerializeField] private BossAnimData animData;
    
        public override EnemyState RunCurrentState() {
            if (!animator.GetBool(animData.hostileAnim.name)) TriggerAnim(animData.hostileAnim);
            Agent.SetDestination(target.transform.position);
            Agent.isStopped = false;

            if (!(Agent.remainingDistance < 4f)) return this;
            
            TriggerAnim(animData.hostileAnim);
            Agent.isStopped = true;
            return nextState;
        }
    }
}
