using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Entities.Enemy.Crawler {
    public class CrawlerHostile : EnemyState {
        [SerializeField] private CrawlerAttack nextState;
        [SerializeField] private AnimSerializedData animData;

        [HideInInspector] public bool canSwitchState = true;
        private AnimParam _currHostileAnim;
        private bool _canSetAnim = true;
        
        public override EnemyState RunCurrentState() {
            if (NavMesh.SamplePosition(target.transform.position, out var hit, Agent.height / 2, NavMesh.AllAreas)) {
                Agent.SetDestination(target.transform.position);
                _canSetAnim = true;
                if (_currHostileAnim.name == null) _currHostileAnim = animData.hostileAnim[0];
                if (!animator.GetBool(_currHostileAnim.name)) TriggerAnim(_currHostileAnim);
            }
        
            else {
                if (_canSetAnim) {
                    _canSetAnim = false;
                    ResetAnim(_currHostileAnim);
                    _currHostileAnim.name = null;
                };
            }
            
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
