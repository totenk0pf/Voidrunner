using System.Collections;
using Core.Events;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
using UI;
using EventType = Core.Events.EventType;

namespace Combat {
    public class GunBase : WeaponBase {
        // [TitleGroup("Gun settings")]
        // public float preshotDelay;
        // public float aftershotDelay;
        // [SerializeField] private int maxAmmo;
        // [SerializeField] private int maxClip;
        // [SerializeField] private LayerMask raycastMask;

        [ReadOnly] protected RangedAttribute attribute; 
        // public Transform firePoint;
        [TitleGroup("Gun states")]
        protected bool isReloading;

        public int currentAmmo;
        public int clipAmount;

        private WeaponType _curWeaponType;
        [TitleGroup("Components")]
        [SerializeField] protected Animator animator;

        protected void Awake() {
            this.AddListener(EventType.RefreshRangedAttributesEvent, param => UpdateAttribute((RangedAttribute)param));
            this.AddListener(EventType.RangedEnemyDamageEvent, dmgData => ApplyDamageOnEnemy((AnimData) dmgData));
            this.AddListener(EventType.WeaponChangedEvent, param => OnWeaponChange((WeaponManager.WeaponEntry) param));
        }

        protected IEnumerator Start() {
            yield return new WaitForSeconds(.5f); //skip X seconds to queue checking after init steps
            if(currentAmmo == 0 || clipAmount == 0)
                NCLogger.Log($"Did not receive refreshAttribute at Awake", LogLevel.ERROR);
        }

        protected void UpdateAttribute(RangedAttribute attribute) {
            this.attribute = attribute;
            
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
            if (Input.GetMouseButtonDown(0) && canAttack && _curWeaponType == WeaponType.Ranged) {
                if (currentAmmo >= 1) {
                    currentAmmo--;
                    this.FireEvent(EventType.WeaponRangedFiredEvent);
                }
            }
            // if (Input.GetKeyDown(KeyCode.R)) {
            //     if (isReloading) return;
            //     if (isAttacking) return;
            //     if (clipAmount >= 1) {
            //         StartCoroutine(Reload());
            //     }
            // }
        }

        protected void ApplyDamageOnEnemy(AnimData dmgData) {
            var enemies = dmgData.Enemies;
            foreach (var enemy in enemies) {
                if (enemies.Count < 1) return;
                Damage(enemy, dmgData.Damage);
                KnockBack(enemy, dmgData.Knockback,(enemy.transform.root.position - dmgData.playerTransform.position).normalized);
                NCLogger.Log($"dmg: {dmgData.Damage}");
                
                UpdateUI();
                this.FireEvent(EventType.WeaponFiredEvent, new WeaponFireUIMsg {
                    type = WeaponType.Ranged,
                    rechargeDuration = attribute.AftershotDelay
                });
               
                this.FireEvent(EventType.WeaponRechargedEvent);
                
            }
        }
        
        // protected IEnumerator Reload() {
        //     isReloading = true;
        //     canAttack = false;
        //     clipAmount--;
        //     yield return new WaitForSeconds(attribute.ReloadTime);
        //     currentAmmo = attribute.MaxAmmo;
        //     isReloading = false;
        //     canAttack = true;
        //     UpdateUI();
        //     yield return null;
        // }

        protected void OnWeaponChange(WeaponManager.WeaponEntry entry) {
            _curWeaponType = entry.Type;
            if (entry.Type != WeaponType.Ranged) return;
            isAttacking = false;
        }
        
    }
}