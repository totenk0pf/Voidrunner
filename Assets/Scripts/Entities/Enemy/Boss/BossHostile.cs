using System;
using Core.Logging;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Entities.Enemy.Boss {
    public class BossHostile : EnemyState
    {
        [SerializeField] private BossAttack nextState;
        [SerializeField] private AnimSerializedData animData;

        [HideInInspector] public bool canSwitchState = true; 
    
        public override EnemyState RunCurrentState() {
            if (!animator.GetBool(animData.hostileAnim[0].name)) TriggerAnim(animData.hostileAnim[0]);
            Agent.SetDestination(target.transform.position);

            if (nextState.inRange && canSwitchState) {
                canSwitchState = false;
                Agent.ResetPath();
                TriggerAnim(animData.hostileAnim[0]);
                return nextState;
            }
            
            return this;
        }
        
        protected override void RestartState() {
            canSwitchState = true;
        }
    }
}
