using System;
using Core.Logging;
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

            NCLogger.Log($"{Agent.remainingDistance} remaining, stopping before {Agent.stoppingDistance}");
            
            if (Agent.pathPending & (Agent.remainingDistance > Agent.stoppingDistance)) return this;
            
            TriggerAnim(animData.hostileAnim);
            return nextState;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Agent.destination, 0.5f);
        }
    }
}
