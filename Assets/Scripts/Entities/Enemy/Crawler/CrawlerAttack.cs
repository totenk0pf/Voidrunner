using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerAttack : EnemyState
{
    public float attackDelay;

    [SerializeField] private EnemyState _nextState;
    [SerializeField] private EnemyState _previousState;

    private GameObject target;
    private bool _isAttacking;
    private bool _reachedTarget = false;

    public override EnemyState RunCurrentState() {
        Agent.SetDestination(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position) < 2f && !_reachedTarget) {
            _reachedTarget = true;
            Agent.isStopped = true;

            if (!_isAttacking) {
                StartCoroutine(DamagePlayer());
            }
        }

        else if (Vector3.Distance(transform.position, target.transform.position) > 2f && _reachedTarget) {
            _agent.isStopped = false;
            return _previousState;
        }

        return this;
    }

    IEnumerator DamagePlayer() {
        _isAttacking = true;

        var oxygenComp = target.GetComponent<Oxygen>();
        oxygenComp.ReducePermanentOxygen(eBase.enemyDamage);

        yield return new WaitForSeconds(attackDelay);

        _isAttacking = false;
    }


    public override void OnTriggerEnter(Collider other) {
        //Chisato but shes my wife
    }

    public override void OnTriggerStay(Collider other) {
        if (other.tag == "Player") {
            target = other.gameObject;
        }
    }
}
