using System.Collections;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine.ProBuilder.MeshOperations;
using Random = UnityEngine.Random;

namespace Entities.Enemy.Boss {

    public class BossAttack : EnemyState {
        public float attackDelay;
        [Space]
        
        [TitleGroup("Attack settings")]
        [SerializeField] private BossHostile previousState;
        [SerializeField] private EnemyState nextState;

        [TitleGroup("Attack sets")]
        [SerializeField] private AnimSerializedData animData;
        
        [Title("Refs")] 
        [SerializeField] private EnemyMoveRootMotion _moveWithRootMotion;

        private bool _canAttack = true;
        private bool _canSwitchState = true;
        private bool _isAttacking = true;

        public override EnemyState RunCurrentState() {
            if (_canSwitchState && !_isAttacking)
            {
                return previousState;
            }
            
            if (_canAttack) {
                StartCoroutine(StartAttack());
            }
            
            return this;
        }

        private IEnumerator StartAttack()
        {
            if (_canSwitchState) yield break;
            _canAttack = false;
            _isAttacking = true;
            if (_moveWithRootMotion.canMove) _moveWithRootMotion.canMove = false;

            //Chance to grab
            //AnimData index 4 is grab attack
            if (Random.Range(0, 1) == 1)
            {
                TriggerAnim(animData.attackAnim[animData.attackAnim.Count]);
            }

            else
            {
                TriggerAnim(animData.attackAnim[Random.Range(0, animData.attackAnim.Count - 1)]);
            }
            
            yield return StartCoroutine(FinishAnimation());
            yield return StartCoroutine(DelayAttack());
        }

        private IEnumerator DelayAttack() {
            yield return new WaitForSeconds(attackDelay);
            StartCoroutine(StartAttack());
        }

        private IEnumerator FinishAnimation() {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
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
                
                inRange = false;
                _canAttack = false;
                _canSwitchState = true;
                _moveWithRootMotion.canMove = true;
                previousState.canSwitchState = true;
            }
        }

        public override void OnTriggerEnter(Collider other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
                inRange = true;
                _canSwitchState = false;
                target = other.gameObject;
            }
        }
    }
}
