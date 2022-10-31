using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerAttack : EnemyState
{
    public float attackDelay;
    
    [SerializeField] private EnemyState _previousState;
    [SerializeField] private EnemyState _nextState;

    private bool _isAttacking = false;

    public override EnemyState RunCurrentState() {
        //Change 2f number if changed in WalkerHostile also 
        if (Vector3.Distance(transform.position, target.transform.position) > 1.4f) {
            return _previousState;
        }

        if (!_isAttacking) {
            StartCoroutine(DamagePlayer());
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
