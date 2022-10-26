using System.Collections;
using Core.Events;
using UnityEngine;
using Sirenix.OdinInspector;
using UI;
using EventType = Core.Events.EventType;

namespace Combat {
    public class GunBase : WeaponBase {
        [TitleGroup("Gun settings")]
        public float preshotDelay;
        public float aftershotDelay;
        public Transform firePoint;
        [SerializeField] private int maxAmmo;
        [SerializeField] private int maxClip;
        [SerializeField] private LayerMask raycastMask;
        
        [TitleGroup("Gun states")]
        protected bool isReloading;

        public int currentAmmo;
        public int clipAmount;

        [TitleGroup("Components")]
        [SerializeField] protected Animator animator;

        protected void Awake() {
            currentAmmo = maxAmmo;
            clipAmount = maxClip;
            UpdateUI();
        }

        protected void UpdateUI() {
            this.FireEvent(EventType.RangedShotEvent, new RangedUIMsg {
                ammo = currentAmmo,
                clip = clipAmount
            });
        }

        protected void Update() {
            if (Input.GetMouseButtonDown(0)) {
                if (!canAttack) return;
                if (isAttacking) return;
                if (currentAmmo >= 1) {
                    currentAmmo--;
                    StartCoroutine(Fire());
                }
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                if (isReloading) return;
                if (isAttacking) return;
                if (clipAmount >= 1) {
                    StartCoroutine(Reload());
                }
            }
        }

        protected override EnemyBase GetEnemy(Collider col = null) {
            Debug.DrawLine(firePoint.position, firePoint.forward * 100f, Color.yellow, 5f);
            if (Physics.Raycast(firePoint.position, firePoint.forward, out var hit, Mathf.Infinity, raycastMask)) {
                var enemy = hit.transform.GetComponent<EnemyBase>();
                return !enemy ? null : enemy;
            }
            return null;
        }

        public override IEnumerator AltFire() {
            yield return null;
        }

        public override IEnumerator Fire() {
            canAttack = false;
            isAttacking = true;
            yield return new WaitForSeconds(preshotDelay);
            Damage(GetEnemy());
            UpdateUI();
            this.FireEvent(EventType.WeaponFiredEvent, new WeaponFireUIMsg {
                type = WeaponType.Ranged,
                rechargeDuration = aftershotDelay
            });
            yield return new WaitForSeconds(aftershotDelay);
            canAttack = true;
            isAttacking = false;
            yield return null;
        }

        protected IEnumerator Reload() {
            isReloading = true;
            canAttack = false;
            clipAmount--;
            currentAmmo = maxAmmo;
            isReloading = false;
            canAttack = true;
            UpdateUI();
            yield return null;
        }
    }
}