using System;
using Core.Logging;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Entities.Enemy.Boss {
    public class BossHostile : BossState
    {
        [SerializeField] private BossAttack nextState;
        [SerializeField] private AnimSerializedData animData;
    
        public override EnemyState RunCurrentState() {
            if (!animator.GetBool(animData.hostileAnim.name)) TriggerAnim(animData.hostileAnim);
            if (Agent.enabled) Agent.SetDestination(target.transform.position);
  
            if (GetPathRemainingDistance(Agent) < 3f && GetPathRemainingDistance(Agent) > -1 && detected)
            {
                TriggerAnim(animData.hostileAnim);
                Agent.isStopped = true;
                nextState.canAttack = true;
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
