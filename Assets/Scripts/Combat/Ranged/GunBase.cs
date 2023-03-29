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
        public Transform firePoint;
        [TitleGroup("Gun states")]
        protected bool isReloading;

        public int currentAmmo;
        public int clipAmount;

        [TitleGroup("Components")]
        [SerializeField] protected Animator animator;

        protected void Awake() {
            this.AddListener(EventType.RefreshRangedAttributesEvent, param => UpdateAttribute((RangedAttribute)param));
            
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

        // protected void Update() {
        //     if (Input.GetMouseButtonDown(0)) {
        //         if (!canAttack) return;
        //         //if (isAttacking) return;
        //         if (currentAmmo >= 1) {
        //             currentAmmo--;
        //             // StartCoroutine(Fire());
        //         }
        //     }
        //     // if (Input.GetKeyDown(KeyCode.R)) {
        //     //     if (isReloading) return;
        //     //     if (isAttacking) return;
        //     //     if (clipAmount >= 1) {
        //     //         StartCoroutine(Reload());
        //     //     }
        //     // }
        // }
        protected void Update() {
            if (Input.GetMouseButtonDown(0) && canAttack) {
                if (currentAmmo >= 1) {
                    currentAmmo--;
                    this.FireEvent(EventType.WeaponRangedFiredEvent);
                }
                //isAttacking = true;
            }
        }
        // protected override EnemyBase GetEnemy(Collider col = null) {
        //     Debug.DrawLine(firePoint.position, firePoint.forward * 100f, Color.yellow, 5f);
        //     if (Physics.Raycast(firePoint.position, firePoint.forward, out var hit, Mathf.Infinity, raycastMask)) {
        //         var enemy = hit.transform.GetComponent<EnemyBase>();
        //         return !enemy ? null : enemy;
        //     }
        //     return null;
        // }

        // public override IEnumerator AltFire() {
        //     yield return null;
        // }
        //
        // public override IEnumerator Fire() {
        //     canAttack = false;
        //     isAttacking = true;
        //     yield return new WaitForSeconds(preshotDelay);
        //     //Damage(GetEnemy());
        //     UpdateUI();
        //     this.FireEvent(EventType.WeaponFiredEvent, new WeaponFireUIMsg {
        //         type = WeaponType.Ranged,
        //         rechargeDuration = aftershotDelay
        //     });
        //     yield return new WaitForSeconds(aftershotDelay);
        //     this.FireEvent(EventType.WeaponRechargedEvent);
        //     canAttack = true;
        //     isAttacking = false;
        //     yield return null;
        // }

        // protected IEnumerator Reload() {
        //     isReloading = true;
        //     canAttack = false;
        //     clipAmount--;
        //     currentAmmo = maxAmmo;
        //     isReloading = false;
        //     canAttack = true;
        //     UpdateUI();
        //     yield return null;
        // }
    }
}