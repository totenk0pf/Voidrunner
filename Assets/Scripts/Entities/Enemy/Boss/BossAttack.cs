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
        private bool _canChangeState;
        private bool _canAttack;
        private bool _isDelayed;

        public override EnemyState RunCurrentState() {
            if (_canChangeState)
            {
                _canChangeState = false;
                return previousState;
            }
            if (!_isDelayed && !_canAttack) {
                StartCoroutine(StartAttack());
            }
            return this;
        }

        public IEnumerator StartAttack()
        {
            _canAttack = false;
            isAttacking = true;
            
            var randomAttack = Random.Range(0, animData.attackAnim.Count);
            var attack = animData.attackAnim[randomAttack];
            
            Agent.enabled = false;
            
            TriggerAnim(attack);
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            
            _canAttack = true;
            isAttacking = false;
            
            var dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist > 2f && !isAttacking)
            {
                Agent.enabled = true;
                Agent.ResetPath();
                _canChangeState = true;
                _canAttack = false;
            }
        }
    }
}
