using System;
using Core.Logging;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Entities.Enemy.Boss {
    public class BossHostile : EnemyState
    {
        [SerializeField] private EnemyState nextState;
        [SerializeField] private BossAnimData animData;
    
        public override EnemyState RunCurrentState() {
            if (!animator.GetBool(animData.hostileAnim.name)) TriggerAnim(animData.hostileAnim);
            Agent.SetDestination(target.transform.position);
  
            if (GetPathRemainingDistance(Agent) < 4f && GetPathRemainingDistance(Agent) > -1)
            {
                TriggerAnim(animData.hostileAnim);
                Agent.isStopped = true;
                return nextState;
            }
            
            Agent.isStopped = false;
            return this;
        }
 
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Agent.destination, 0.5f);
        }
    }
}
