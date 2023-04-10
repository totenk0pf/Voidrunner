using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
using UI;
using EventType = Core.Events.EventType;

namespace Combat {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class MeleeBase : WeaponBase {
        // [TitleGroup("Melee settings")]
        // public float attackSpeed;
        // public float attackRadius;
        // public float attackSpeedModifier = 1f;
        //
        // protected List<EnemyBase> enemies = new();
        
        protected void Awake() {
            this.AddListener(EventType.MeleeEnemyDamageEvent, dmgData => ApplyDamageOnEnemy((AnimData) dmgData));
            this.AddListener(EventType.WeaponChangedEvent, param => OnWeaponChange((WeaponManager.WeaponEntry) param));
            canAttack = true;
            // StartCoroutine(Fire());
            // StartCoroutine(AltFire());
        }

        protected void Update() {
            if (Input.GetMouseButtonDown(0) && canAttack) {
                this.FireEvent(EventType.WeaponMeleeFiredEvent);
                //isAttacking = true;
            }
        }

        protected void ApplyDamageOnEnemy(AnimData dmgData) {
            if(dmgData == null) return;
            var enemies = dmgData.Enemies;
            if (enemies == null) return;
            foreach (var enemy in enemies) {
                if (enemies.Count < 1) return;
                Damage(enemy, dmgData.Damage);
                KnockBack(enemy, dmgData.Knockback,(enemy.transform.root.position - dmgData.playerTransform.position).normalized);
                NCLogger.Log($"dmg: {dmgData.Damage}");
            }
            
            this.FireEvent(EventType.WeaponFiredEvent, new WeaponFireUIMsg {
                    type = WeaponType.Melee,
                    rechargeDuration = dmgData.animDuration //default value
            });
            //waiting for recharge time is handled in CombatManager
            this.FireEvent(EventType.WeaponRechargedEvent);
            //resetting atk attributes is handled in CombatManager
        }
        
        protected void OnWeaponChange(WeaponManager.WeaponEntry entry) {
            if (entry.Type != WeaponType.Melee) return;
            StopAllCoroutines();
            isAttacking = false;
        }
    }
}