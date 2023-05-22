using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
using UI;
using EventType = Core.Events.EventType;

namespace Combat {
    public class MeleeBase : WeaponBase {

        protected void Awake() {
            base.Awake();
            this.AddListener(EventType.MeleeEnemyDamageEvent, dmgData => ApplyDamageOnEnemy((AnimData) dmgData));
            
            canAttack = true;
        }

        protected void Update() {
            if (Input.GetKeyDown(entry.key) && canAttack && entry.type == WeaponType.Melee) {

                this.FireEvent(EventType.WeaponMeleeFiredEvent);
            }
        }

        protected void ApplyDamageOnEnemy(AnimData dmgData) {
            if(dmgData == null) return;
            var enemies = dmgData.Enemies;
            if (enemies == null) return;
            foreach (var enemy in enemies) {
                if (enemies.Count < 1) return;
                var playerToEnemyVector3 = (enemy.transform.root.position - dmgData.playerTransform.position);
                var knockbackDir = playerToEnemyVector3.magnitude <= 1
                    ? dmgData.playerTransform.forward.normalized
                    : playerToEnemyVector3.normalized;
                knockbackDir.y = 0;
                Damage(enemy, dmgData.Damage);
                KnockBack(enemy, dmgData.KnockbackDuration, knockbackDir * dmgData.KnockbackRange);
                //NCLogger.Log($"dmg: {dmgData.Damage}");
            }
            
            this.FireEvent(EventType.WeaponFiredEvent, new WeaponFireUIMsg {
                    type = WeaponType.Melee,
                    rechargeDuration = dmgData.animDuration //default value
            });
            //waiting for recharge time is handled in CombatManager
            this.FireEvent(EventType.WeaponRechargedEvent);
            //resetting atk attributes is handled in CombatManager
        }
    }
}