using System.Collections;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;

namespace Entities.Enemy.Juggernaut {
    public class JuggernautAttack : EnemyState
    {
        public float attackDelay;
        [HideInInspector] public bool inRange; 
    
        [TitleGroup("Attack settings")]
        [SerializeField] private JuggernautHostile _previousState;
        [SerializeField] private EnemyState _nextState;
        
        [Title("Refs")] 
        [SerializeField] private EnemyMoveRootMotion _moveWithRootMotion;

        [TitleGroup("Attack sets")] [SerializeField]
        private AnimSerializedData animData;

        private bool _isAttacking = false;
        private bool _canSwitchState = false;
        private bool _canAttack = true;

        public override EnemyState RunCurrentState() {
            if (_canSwitchState && !_isAttacking) {
                return _previousState;
            }

            if (_canAttack) {
                StartAttack();
            }

            return this;
        }
        
        private void StartAttack() {
            _canAttack = false;
            _isAttacking = true;
            if (_moveWithRootMotion.canMove) _moveWithRootMotion.canMove = false;
            var randomAttack = Random.Range(0, animData.attackAnim.Count);
            var attack = animData.attackAnim[randomAttack];
            TriggerAnim(attack);
        }

        public void ResetAttack() {
            _isAttacking = false;
            if (_canSwitchState) return;
            StartCoroutine(DelayAttack());
        }

        private IEnumerator DelayAttack() {
            yield return new WaitForSeconds(attackDelay);
            StartAttack();
        }

        protected override void RestartState() {
            StopAllCoroutines();
            foreach (var anim in animData.attackAnim) {
                ResetAnim(anim);
            }
                
            _canAttack = false;
            inRange = false;
        }

        public override void OnTriggerExit(Collider other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
                StopAllCoroutines();

                foreach (var anim in animData.attackAnim) {
                    ResetAnim(anim);
                }

                _isAttacking = false;
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
