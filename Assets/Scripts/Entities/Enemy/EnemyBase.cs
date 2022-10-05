using System.Collections;
using System.Collections.Generic;

public class EnemyBase : EntityBase
{
    public float enemyDamage, enemySpeed; 

    public virtual void Attack() {
        throw new System.NotImplementedException();
    }

    public virtual void Move() {
        throw new System.NotImplementedException();
    } 
}
