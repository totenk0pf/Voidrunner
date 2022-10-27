using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat {
    public class WeaponEmpowerment : MonoBehaviour {
        public enum EmpowerType {
            None,
            Fire, //bonus dmg to walker
            Electric, //stun crawler
            Acid //DoT juggernaut
        }

        [TitleGroup("Empower settings")] private EmpowerType _currentEmpowerType = EmpowerType.None;
        private WeaponBase _currentWeapon; //current weapon that's equipped != not necessarily empowered weapon
        [SerializeField] private Dictionary<EmpowerType, KeyCode> empowerKeys = new();
        [SerializeField] private Dictionary<EnemyType, EmpowerType> counterType = new();
        private WeaponBase _empoweredWeapon;
        [SerializeField] private float duration;

        [TabGroup("Fire")] [SerializeField] private float fireDamage; 

        [TabGroup("Electric")] [SerializeField] private float stunDuration;

        [TabGroup("Acid")] [SerializeField] private float acidDamagePerInterval;
        [TabGroup("Acid")] [SerializeField] private int intervalNum;
        [TabGroup("Acid")] [SerializeField] private float intervalDuration;

        private void Awake() {
            EventDispatcher.Instance.AddListener(EventType.WeaponChangedEvent,
                                                 param => UpdateCurrentWeapon((WeaponEntry) param));
            EventDispatcher.Instance.AddListener(EventType.DamageEnemyEvent,
                                                 enemy => DealEmpowerDamage((EnemyBase) enemy));
        }

        private void Update() {
            UpdateEmpowerStatus();
        }

        private void UpdateEmpowerStatus() {
            if (_currentEmpowerType != EmpowerType.None) return;
            foreach (var item in empowerKeys) {
                if (Input.GetKeyDown(item.Value)) {
                    StartCoroutine(EnableEmpowerRoutine(item.Key));
                }
            }
        }

        private void DealEmpowerDamage(EnemyBase enemy) {
            if (_currentWeapon != _empoweredWeapon) return;
            if (!CanDamage(enemy)) return;
            switch (_currentEmpowerType) {
                case EmpowerType.None:
                    break;
                case EmpowerType.Fire:
                    enemy.TakeDamage(fireDamage);
                    break;
                case EmpowerType.Electric:
                    enemy.Stun(stunDuration);
                    break;
                case EmpowerType.Acid:
                    enemy.TakeTickDamage(acidDamagePerInterval, intervalDuration, intervalNum);
                    break;
            }
        }

        private bool CanDamage(EnemyBase enemy) => _currentEmpowerType == counterType[enemy.type];

        private void UpdateCurrentWeapon(WeaponEntry weapon) {
            _currentWeapon = weapon.reference;
        }

        private IEnumerator EnableEmpowerRoutine(EmpowerType type) {
            _empoweredWeapon = _currentWeapon;
            _currentEmpowerType = type;
            yield return new WaitForSeconds(duration);
            _empoweredWeapon = null;
            _currentEmpowerType = EmpowerType.None;
        }
    }
}