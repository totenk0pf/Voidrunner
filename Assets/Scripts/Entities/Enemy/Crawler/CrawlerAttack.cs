using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerAttack : EnemyState
{
    public float attackDelay;

    [SerializeField] private EnemyState _nextState;
    [SerializeField] private EnemyState _previousState;
    
    private bool _isAttacking;
    private bool _reachedTarget = false;

    public override EnemyState RunCurrentState() {
        Agent.SetDestination(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position) < 2.5f && !_reachedTarget) {
            _reachedTarget = true;
            Agent.isStopped = true;
        }

        else if (Vector3.Distance(transform.position, target.transform.position) > 2.5f && _reachedTarget)
        {
            _agent.isStopped = false;
            return _previousState;
        }

        if (_reachedTarget)
        {
            if (!_isAttacking) {
                StartCoroutine(DamagePlayer());
            }
        }

        return this;
    }

    IEnumerator DamagePlayer() {
        _isAttacking = true;

        var oxygenComp = target.GetComponent<Oxygen>();
        oxygenComp.ReducePermanentOxygen(enemyBase.enemyDamage);

        yield return new WaitForSeconds(attackDelay);

        _isAttacking = false;
    }
}
