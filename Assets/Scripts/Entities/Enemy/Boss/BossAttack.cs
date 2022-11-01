using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Logging;
using Random = UnityEngine.Random;

namespace Entities.Enemy.Boss {
    public enum AttackState {
        Swing,
        Slam,
        Stomp,
        Lunge
    }
    
    public class BossAttack : EnemyState
    {
        public float attackDelay;
    
        [SerializeField] private EnemyState previousState;
        [SerializeField] private EnemyState nextState;

        private readonly List<AttackState> _attackSets = new() 
            { AttackState.Swing, AttackState.Slam, AttackState.Stomp, AttackState.Lunge};

        private bool _isAttacking;

        public override EnemyState RunCurrentState() {
            //Change 2f number if changed in WalkerHostile also 
            if (Vector3.Distance(transform.position, target.transform.position) > 4f) {
                return previousState;
            }

            if (!_isAttacking) {
                StartCoroutine(DamagePlayer());
            }

            return this;
        }

        IEnumerator DamagePlayer(){
            _isAttacking = true;
            
            var randomAttack = Random.Range(0, _attackSets.Count);
            var triggerName = _attackSets[randomAttack].ToString();

            var oxygenComp = target.GetComponent<Oxygen>();
            oxygenComp.ReducePermanentOxygen(enemyBase.enemyDamage);

            yield return new WaitForSeconds(attackDelay);

            _isAttacking = false;
        }
    }
}
