using System.Collections;
using Core.Events;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
using UI;
using EventType = Core.Events.EventType;

namespace Combat {
    public class GunBase : WeaponBase {
        [TitleGroup("Ranged Specifics")]
        [SerializeField] protected RangedData rangedData;
        protected RangedAttribute attribute;
        
        public int currentAmmo;
        public int clipAmount;
        private float _cooldown;
        private float _cooldownTimeStamp = 0;

        protected void Awake() {
            base.Awake();
            this.AddListener(EventType.RangedEnemyDamageEvent, dmgData => ApplyDamageOnEnemy((AnimData) dmgData));

            if(!rangedData) NCLogger.Log($"rangedData missing reference", LogLevel.ERROR);
            attribute = rangedData.Attribute;
            
            canAttack = true;
            UpdateAttribute();
        }

        protected void Start() {
            //yield return new WaitForSeconds(.5f); //skip X seconds to queue checking after init steps
            if (currentAmmo == 0 || clipAmount == 0)
            {
                NCLogger.Log($"Did not receive refreshAttribute at Awake", LogLevel.ERROR);
            }
            _cooldown = attribute.fireClip.length / attribute.AtkSpdModifier + attribute.AftershotDelay;
        }

        protected void UpdateAttribute() {

            currentAmmo = this.attribute.MaxAmmo;
            clipAmount = this.attribute.MaxClip;
            UpdateUI();
        }
        
        protected void UpdateUI() {
            this.FireEvent(EventType.WeaponPostRangedFiredEvent, new RangedUIMsg {
                ammo = currentAmmo,
                clip = clipAmount
            });
        }
        
        protected void Update() {
            if (Input.GetKeyDown(entry.key) && canAttack && entry.type == WeaponType.Ranged) {
                if (Time.time > _cooldownTimeStamp) {
                    _cooldown = attribute.fireClip.length / attribute.AtkSpdModifier + attribute.AftershotDelay;
                    _cooldownTimeStamp = Time.time + _cooldown; 
                    if (currentAmmo >= 1) {
                        currentAmmo--;
                        this.FireEvent(EventType.WeaponRangedFiredEvent);
                    }
                }
            }
        }

        protected void ApplyDamageOnEnemy(AnimData dmgData) {
            UpdateUI();
            this.FireEvent(EventType.WeaponFiredEvent, new WeaponFireUIMsg {
                type = WeaponType.Ranged,
                rechargeDuration = _cooldown
            });
               
            this.FireEvent(EventType.WeaponRechargedEvent);
            
            var enemies = dmgData.EnemiesToCount;
            foreach (var enemy in enemies) {
                if (enemies.Count < 1) return;
                if (!enemy.Key) return;
                
                var playerToEnemyVector3 = (enemy.Key.transform.root.position - dmgData.playerTransform.position);
                var knockbackDir = playerToEnemyVector3.magnitude <= 1
                    ? dmgData.playerTransform.forward.normalized
                    : playerToEnemyVector3.normalized;
                knockbackDir.y = 0;
                
                Damage(enemy.Key, dmgData.Damage * enemy.Value, dmgData.playerTransform);
                var range = Mathf.Clamp(dmgData.KnockbackRange * enemy.Value, dmgData.KnockbackRange,
                    dmgData.KnockbackCap);
                KnockBack(enemy.Key, dmgData.KnockbackDuration, knockbackDir * range);
            }
        }
    }
}