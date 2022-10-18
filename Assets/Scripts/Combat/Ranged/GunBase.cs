using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Combat {
    public class GunBase : WeaponBase {
        [TitleGroup("Gun settings")]
        public float preshotDelay;
        public Transform firePoint;
        [SerializeField] private int maxAmmo;
        [SerializeField] private int maxClip;
        
        [TitleGroup("Gun states")]
        protected bool isReloading;
        protected bool isFiring;

        public int currentAmmo;
        public int clipAmount;

        [TitleGroup("Components")]
        [SerializeField] protected Animator animator;

        protected void Awake() {
            currentAmmo = maxAmmo;
        }

        protected void Update() {
            if (Input.GetMouseButtonDown(2) && animator.GetCurrentAnimatorStateInfo(0).IsName("GunSwitching")) {
                if (isFiring) return;
                if (currentAmmo >= 1) {
                    currentAmmo--;
                    StartCoroutine(Fire());
                }
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                if (isReloading) return;
                if (clipAmount >= 1) {
                    isReloading = true;
                    StartCoroutine(Reload());
                }
            }
        }

        protected EnemyBase GetEnemy() {
            if (Physics.Raycast(firePoint.position, firePoint.forward, out var hit, Mathf.Infinity)) {
                var enemy = hit.transform.GetComponent<EnemyBase>();
                return !enemy ? null : enemy;
            }
            return null;
        }

        public override IEnumerator AltFire() {
            yield return null;
        }

        public override IEnumerator Fire() {
            yield return new WaitForSeconds(preshotDelay);
            Damage(GetEnemy());
            isFiring = false;
            yield return null;
        }

        protected IEnumerator Reload() {
            yield return new WaitForSeconds(transform.GetComponentInParent<Animator>().runtimeAnimatorController
                                                .animationClips[0].length);
            clipAmount--;
            currentAmmo = maxAmmo;
            isReloading = false;
            yield return null;
        }
    }
}