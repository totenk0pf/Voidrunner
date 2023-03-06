using System.Collections;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
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

        public bool isAttacking;
        public bool canAttack = true;

        public override EnemyState RunCurrentState() {
            if (Vector3.Distance(transform.position, target.transform.position) > 2.4f && !isAttacking)
            {
                Agent.enabled = true;
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
            
            Agent.enabled = false;
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);

            if (Vector3.Distance(transform.position, target.transform.position) > 2.4f)
            {
                isAttacking = false;
                StopCoroutine(StartAttack());
                yield return null;
            }

            else
            {
                yield return DelayAttack();
            }
        }

        public IEnumerator DelayAttack()
        {
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(StartAttack());
        }
    }
}
