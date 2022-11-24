using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Enemy.Juggernaut {
    public class JuggernautAttack : EnemyState
    {
        public float attackDelay;
    
        [TitleGroup("Attack settings")]
        [SerializeField] private EnemyState _previousState;
        [SerializeField] private EnemyState _nextState;

        [TitleGroup("Attack sets")] [SerializeField]
        private AnimSerializedData animData; 

        private bool _isAttacking;
        private bool _canChangeState;
        private bool _canAttack = true;

        public override EnemyState RunCurrentState() {
            if (_canChangeState) {
                _canChangeState = false;    
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
            
            Agent.enabled = false;
            
            TriggerAnim(attack);
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            
            _canAttack = true;
            _isAttacking = false;

            var dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist > 2f && !_isAttacking)
            {
                Agent.enabled = true;
                Agent.ResetPath();
                _canChangeState = true;
                _canAttack = false;
            }

            else yield return DelayAttack();
        }

        private IEnumerator DelayAttack()
        {
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(StartAttack());
        }

        //TODO: Spin Attack for juggernaut
        //TODO: Slam Attack for juggernaut
    
    }
}
