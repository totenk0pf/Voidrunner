using System.Collections;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine.ProBuilder.MeshOperations;
using Random = UnityEngine.Random;

namespace Entities.Enemy.Boss {

    public class BossAttack : BossState
    {
        [TitleGroup("Attack settings")]
    
        [SerializeField] private EnemyState previousState;
        [SerializeField] private EnemyState nextState;

        [TitleGroup("Attack sets")]
        [SerializeField] private AnimSerializedData animData;

        public bool canAttack = true;
        public bool canSwitchState = true;
        public bool isAttacking = true;

        public override EnemyState RunCurrentState() {
            if (canSwitchState && !isAttacking)
            {
                Agent.ResetPath();
                canAttack = false;
                return previousState;
            }
            
            if (canAttack) {
                StartCoroutine(StartAttack());
            }
            
            return this;
        }

        public IEnumerator StartAttack()
        {
            canAttack = false;
            isAttacking = true;

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
            
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            isAttacking = false;
            yield return DelayAttack();
        }

        public IEnumerator DelayAttack() {
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(StartAttack());
        }

        public override void OnTriggerExit(Collider other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
                canSwitchState = true;
                StopCoroutine(StartAttack());
            }
        }

        public override void OnTriggerEnter(Collider other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) {
                canSwitchState = false;
                target = other.gameObject;
            }
        }
    }
}
