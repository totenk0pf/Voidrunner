using System.Collections;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using Sirenix.OdinInspector;
using UI;
using EventType = Core.Events.EventType;

namespace Combat {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class MeleeBase : WeaponBase {
        [TitleGroup("Melee settings")]
        public float attackSpeed;
        public float attackRadius;
        public float attackSpeedModifier = 1f;

        protected List<EnemyBase> enemies = new();

        protected void Awake() {
            canAttack = true;
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
            var enemy = GetEnemy(col);
            if (enemy) enemies.Add(enemy);
        }

        protected void OnTriggerExit(Collider col) {
            var enemy = GetEnemy(col);
            if (enemy) enemies.Remove(enemy);
        }

        protected override EnemyBase GetEnemy(Collider col) {
            var enemy = col.GetComponent<EnemyBase>();
            return !enemy || enemies.Contains(enemy) ? null : enemy;
        }
        
        public override IEnumerator Fire() {
            while (true) {
                if (isAttacking) {
                    enemies.RemoveAll(x => !x);
                    if (enemies.Count < 1) yield return null;
                    canAttack = false;
                    for (int i = 0; i < enemies.Count; i++) {
                        if (Vector3.Distance(enemies[i].transform.position, transform.position) > attackRadius) continue;
                        Damage(enemies[i]);
                    }
                    var rechargeTime = attackSpeed / attackSpeedModifier;
                    this.FireEvent(EventType.WeaponFiredEvent, new WeaponFireUIMsg {
                        type = WeaponType.Melee,
                        rechargeDuration = rechargeTime
                    });
                    yield return new WaitForSeconds(rechargeTime);
                    this.FireEvent(EventType.WeaponRechargedEvent);
                    canAttack = true;
                    isAttacking = false;
                    yield return null;
                }
                yield return null;
            }
        }

        public override IEnumerator AltFire() {
            yield return null;
        }
    }
}