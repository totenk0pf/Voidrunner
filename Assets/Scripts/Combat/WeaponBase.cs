using System.Collections;
using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon {
        // public float damage;
        // public float damageScale;
        // public float damageModifier;
        
        public bool canAttack;
        public bool isAttacking;
        
        // public abstract IEnumerator Fire();
        // public abstract IEnumerator AltFire();
        //protected abstract EnemyBase GetEnemy(Collider col = null);
        protected virtual void Damage(EnemyBase enemy, float damage) {
            if (!enemy) return;
            enemy.TakeDamage(damage);
            
            EventDispatcher.Instance.FireEvent(EventType.EmpowerDamageEnemyEvent, enemy);
        }
    }
}