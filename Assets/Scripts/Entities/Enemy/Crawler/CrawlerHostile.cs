using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Enemy.Crawler {
    public class CrawlerHostile : EnemyState {
        [SerializeField] private CrawlerAttack nextState;
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
    }
}
