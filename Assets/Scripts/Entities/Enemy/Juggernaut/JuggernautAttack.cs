using System.Collections;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;

namespace Entities.Enemy.Juggernaut {
    public class JuggernautAttack : EnemyState
    {
        public float attackDelay;
    
        [TitleGroup("Attack settings")]
        [SerializeField] private JuggernautHostile _previousState;
        [SerializeField] private EnemyState _nextState;

        [TitleGroup("Attack sets")] [SerializeField]
        private AnimSerializedData animData;
        
        public bool inRange;

        private bool _isAttacking = false;
        private bool _canSwitchState = false;
        private bool _canAttack = true;

        public override EnemyState RunCurrentState() {
            if (_canSwitchState && !_isAttacking) {
                return _previousState;
            }

            if (_canAttack) {
                StartCoroutine(StartAttack());
            }

            return this;
        }
        
        IEnumerator StartAttack() {
            _canAttack = false;
            _isAttacking = true;
            
            var randomAttack = Random.Range(0, animData.attackAnim.Count);
            var attack = animData.attackAnim[randomAttack];
            TriggerAnim(attack);

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            _isAttacking = false;
            yield return DelayAttack();
        }

        private IEnumerator DelayAttack()
        {
            yield return new WaitForSeconds(attackDelay);
            yield return StartCoroutine(StartAttack());
        }

        public override void OnTriggerExit(Collider other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
                StopAllCoroutines();
                
                Agent.ResetPath();
                _canAttack = false;
                inRange = false;
            
                _canSwitchState = true;
            }
        }

        public override void OnTriggerEnter(Collider other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
                target = other.gameObject;
                inRange = true;
                _canAttack = true;
           
                _canSwitchState = false;
            }
        }
    }
}
