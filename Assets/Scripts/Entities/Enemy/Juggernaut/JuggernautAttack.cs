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
        
        [Title("Refs")] 
        [SerializeField] private EnemyMoveRootMotion _moveWithRootMotion;

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
            if (_moveWithRootMotion.canMove) _moveWithRootMotion.canMove = false;
            var randomAttack = Random.Range(0, animData.attackAnim.Count);
            var attack = animData.attackAnim[randomAttack];
            TriggerAnim(attack);
            yield return StartCoroutine(FinishAnimation());
            yield return StartCoroutine(DelayAttack());
        }

        private IEnumerator DelayAttack()
        {
            yield return new WaitForSeconds(attackDelay);
            StartCoroutine(StartAttack());
        }
        
        private IEnumerator FinishAnimation() {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
            _isAttacking = false;
        }

        public override void OnTriggerExit(Collider other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
                StopAllCoroutines();
                if (_isAttacking) {
                    StartCoroutine(FinishAnimation());
                }

                foreach (var anim in animData.attackAnim) {
                    ResetAnim(anim);
                }
                
                _canAttack = false;
                inRange = false;
                
                _moveWithRootMotion.canMove = true;
                _canSwitchState = true;
                _previousState.canSwitch = true;
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
