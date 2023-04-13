using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Logging;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon {
        // public float damage;
        // public float damageScale;
        // public float damageModifier;
        [SerializeField] protected WeaponEntriesData EntriesData;
        [SerializeField] protected WeaponType type;
        protected WeaponEntry entry;
        public bool canAttack;
        public bool isAttacking;
        
        protected void Awake()
        {
            if(!EntriesData) NCLogger.Log($"Missing Entries Data", LogLevel.ERROR);    
            //this.AddListener(EventType.InitWeaponRefEvent, param => InitWeaponRef( (List<WeaponEntry>) param));
            entry = EntriesData.HookReference(this, type);
        }

        protected virtual void Damage(EnemyBase enemy, float damage) {
            if (!enemy) return;
            enemy.TakeDamage(damage);
            
            EventDispatcher.Instance.FireEvent(EventType.EmpowerDamageEnemyEvent, enemy);
        }

        protected virtual void KnockBack(EnemyBase enemy, float force, Vector3 dir) {
            if (!enemy) return;
            enemy.Rigidbody.AddForce(dir * force, ForceMode.Impulse);
        }

        public void InitWeaponRef(WeaponEntry entry) {
            this.entry = entry;
        }
    }
}