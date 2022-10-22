using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Combat {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class MeleeBase : WeaponBase {
        [TitleGroup("Melee settings")]
        public float attackSpeed;
        public float attackRadius;
        public float attackSpeedModifier = 1f;
        
        [TitleGroup("Melee states")]
        protected bool isAttacking;
        protected bool canAttack;

        protected List<EnemyBase> enemies = new();

        protected void Awake() {
            StartCoroutine(Fire());
            StartCoroutine(AltFire());
        }

        protected void Update() {
            if (Input.GetMouseButtonDown(0)) {
                if (canAttack) {
                    isAttacking = true;
                }
            }
        }

        protected void OnTriggerEnter(Collider col) {
            var enemy = col.GetComponent<EnemyBase>();
            if (!enemy) return;
            enemies.Add(enemy);
        }

        protected void OnTriggerExit(Collider col) {
            var enemy = col.GetComponent<EnemyBase>();
            if (!enemy) return;
            if (enemies.Contains(enemy)) enemies.Remove(enemy);
        }

        public override IEnumerator Fire() {
            if (isAttacking) {
                canAttack = false;
                for (int i = 0; i < enemies.Count; i++) {
                    if (Vector3.Distance(enemies[i].transform.position, transform.position) > attackRadius) continue;
                    Damage(enemies[i]);
                }

                yield return new WaitForSeconds(attackSpeed / attackSpeedModifier);
                canAttack = true;
            }

            yield return null;
        }

        public override IEnumerator AltFire() {
            yield return null;
        }
    }
}