using System.Collections;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
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

        public override EnemyState RunCurrentState() {
            //Change 2f number if changed in WalkerHostile also 
            if (Vector3.Distance(transform.position, target.transform.position) > 3.2f & !isAttacking) {
                return previousState;
            }
            if (!isAttacking) StartCoroutine(StartAttack());
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
