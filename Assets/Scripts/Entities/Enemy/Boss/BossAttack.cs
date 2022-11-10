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
            if (Agent.enabled) {
                Agent.SetDestination(target.transform.position);
            }
            
            //Change 2f number if changed in WalkerHostile also 
            if (GetPathRemainingDistance(Agent) > 4f && GetPathRemainingDistance(Agent) > -1 && !isAttacking) {
                Debug.Log("Condition Met");
                return previousState;
            }
            if (!isAttacking) {
                TriggerAnim(animData.hostileAnim);
                StartCoroutine(StartAttack());
            }
            
            return this;
        }

        public IEnumerator StartAttack() {
            isAttacking = true;
            var randomAttack = Random.Range(0, animData.attackAnim.Count);
            var attack = animData.attackAnim[randomAttack];
            Agent.enabled = false;
            TriggerAnim(attack);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            isAttacking = false;
            Agent.enabled = true;
            Agent.ResetPath();
        }
    }
}
