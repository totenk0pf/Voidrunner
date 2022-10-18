using System.Collections;
using UnityEngine;

namespace Combat
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon {
        public float damage;
        public abstract IEnumerator Fire();
        public abstract IEnumerator AltFire();
        protected virtual void Damage(EnemyBase enemy) {
            if (!enemy) return;
            enemy.TakeDamage(damage);
        }
    }
}