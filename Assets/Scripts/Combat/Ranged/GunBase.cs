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
        [SerializeField] private LayerMask raycastMask;
        
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
            if (Input.GetMouseButtonDown(0)) {
                if (isFiring) return;
                if (currentAmmo >= 1) {
                    currentAmmo--;
                    StartCoroutine(Fire());
                }
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                if (isReloading) return;
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
            isFiring = true;
            yield return new WaitForSeconds(preshotDelay);
            Damage(GetEnemy());
            isFiring = false;
            yield return null;
        }

        protected IEnumerator Reload() {
            isReloading = true;
            // yield return new WaitForSeconds(transform.GetComponentInParent<Animator>().runtimeAnimatorController.animationClips[0].length);
            clipAmount--;
            currentAmmo = maxAmmo;
            isReloading = false;
            yield return null;
        }
    }
}