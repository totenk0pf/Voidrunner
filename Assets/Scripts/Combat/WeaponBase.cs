using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Logging;
using DG.Tweening;
using Player.Audio;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon {
        public PlayerAudioPlayer audioPlayer;
        [SerializeField] protected WeaponEntriesData EntriesData;
        [SerializeField] protected WeaponType type;
        protected WeaponEntry entry;
        public bool canAttack;
        public bool isAttacking;
        
        protected void Awake()
        {
            if(!EntriesData) NCLogger.Log($"Missing Entries Data", LogLevel.ERROR);
            entry = EntriesData.HookReference(this, type);
        }

        protected virtual void Damage(EnemyBase enemy, float damage, Transform playerT) {
            if (!enemy) return;
            enemy.TakeDamage(damage);
            this.FireEvent(EventType.SpawnBloodEvent, 
                new ParticleCallbackData(( playerT.position - enemy.transform.position).normalized,
                    enemy.transform.position + enemy.transform.forward * .5f + enemy.transform.up * .3f,
                    enemy.transform));
            EventDispatcher.Instance.FireEvent(EventType.EmpowerDamageEnemyEvent, enemy);
        }

        protected virtual void KnockBack(EnemyBase enemy, float duration, Vector3 dir) {
            if (!enemy) return;
            enemy.OnKnockback(dir, duration);
        }
    }
}