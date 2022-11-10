using System.Collections;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.ProBuilder.MeshOperations;
using Random = UnityEngine.Random;

namespace Entities.Enemy.Boss {

    public class BossAttack : EnemyState
    {
        [TitleGroup("Attack settings")]
    
        [SerializeField] private EnemyState previousState;
        [SerializeField] private EnemyState nextState;

        [TitleGroup("Attack sets")]
        [SerializeField] private BossAnimData animData;

        public bool isAttacking;
        public bool canAttack = true;

        public override EnemyState RunCurrentState() {
            if (!isAttacking) {
                StartCoroutine(StartAttack());
            }
            //Change 2f number if changed in WalkerHostile also 
            if (Vector3.Distance(transform.position, target.transform.position) > 3.2f && !isAttacking) {
                return previousState;
            }
            return this;
        }

        public IEnumerator StartAttack() {
            isAttacking = true;
            var randomAttack = Random.Range(0, animData.attackAnim.Count);
            var attack = animData.attackAnim[randomAttack];
            TriggerAnim(attack);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            isAttacking = false;
        }
    }
}
