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
        [HideInInspector] public bool inRange; 
        [Space]
        
        [TitleGroup("Attack settings")]
        [SerializeField] private BossHostile previousState;
        [SerializeField] private EnemyState nextState;

        [TitleGroup("Attack sets")]
        [SerializeField] private EnemyMovelistData animData;
        [SerializeField] private AnimationClip idleAnim;
        
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
                StartAttack();
            }
            
            return this;
        }

        private void StartAttack() {
            _canAttack = false;
            _isAttacking = true;
            if (_moveWithRootMotion.canMove) _moveWithRootMotion.canMove = false;
            TriggerAnim(GetItemFromMoveList(animData.moveList));
        }

        public void RestartAttack() {
            _isAttacking = false;
            if (_canSwitchState) return;
            StartCoroutine(DelayAttack());
        }

        private IEnumerator DelayAttack() {
            yield return new WaitForSeconds(attackDelay);
            StartAttack();
        }
        
        public override void OnTriggerExit(Collider other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
                StopAllCoroutines();

                foreach (var move in animData.moveList) {
                    ResetAnim(move.anim);
                }

                _isAttacking = false;
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
                _canAttack = true;
                _canSwitchState = false;
                target = other.gameObject;
            }
        }
    }
}
