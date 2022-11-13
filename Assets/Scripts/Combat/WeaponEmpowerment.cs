using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat {
    public enum EmpowerType {
        None,
        Fire, //bonus dmg to walker
        Electric, //stun crawler
        Acid //DoT juggernaut
    }

    [Serializable]
    public struct EmpowerData {
        public EmpowerType type;
        public EnemyType counterType;
        public KeyCode key;
        public float chargeDuration;
    }
    
    public class WeaponEmpowerment : MonoBehaviour {

        [TitleGroup("Empower settings")] 
        private EmpowerType _currentEmpowerType = EmpowerType.Acid;
        private WeaponBase _currentWeapon; //current weapon that's equipped != not necessarily empowered weapon
        private bool _canEmpower;
        private bool _canChangeEmpower;

        [SerializeField] private List<EmpowerData> empowerData;
        private int _empowerIndex;
        
        private WeaponBase _empoweredWeapon;
        [SerializeField] private float duration;

        [TabGroup("Fire")] [SerializeField] private float fireDamage; 

        [TabGroup("Electric")] [SerializeField] private float stunDuration;

        [TabGroup("Acid")] [SerializeField] private float acidDamagePerInterval;
        [TabGroup("Acid")] [SerializeField] private int intervalNum;
        [TabGroup("Acid")] [SerializeField] private float intervalDuration;

        private void Awake() {
            _empowerIndex = 0;
            EventDispatcher.Instance.AddListener(EventType.WeaponChangedEvent,
                                                 param => UpdateCurrentWeapon((WeaponEntry) param));
            EventDispatcher.Instance.AddListener(EventType.DamageEnemyEvent,
                                                 enemy => DealEmpowerDamage((EnemyBase) enemy));
            EventDispatcher.Instance.AddListener(EventType.InventoryToggleEvent, msg => OnInventoryUIEvent((InventoryToggleMsg) msg));
            UpdateCurrentAugment();
        }

        private void Update() {
            if (!_canChangeEmpower) return;
            UpdateCurrentAugment();
            UpdateEmpowerStatus();
        }

        private void UpdateEmpowerStatus() {
            if (_currentEmpowerType != EmpowerType.None) return;
            if (!_canEmpower) return;
            foreach (var item in empowerData) {
                if (Input.GetKeyDown(item.key)) {
                    StartCoroutine(EnableEmpowerRoutine(item.type));
                }
            }
        }

        private void ResetEmpower(EmpowerData newType) {
            StartCoroutine(ResetEmpowerRoutine(newType));
        }
        
        private IEnumerator ResetEmpowerRoutine(EmpowerData newType) {
            _canEmpower       = false;
            _canChangeEmpower = false;
            yield return new WaitForSeconds(newType.chargeDuration);
            _canEmpower       = true;
            _canChangeEmpower = true;
            yield return null;
        }

        private void UpdateCurrentAugment() {
            var scroll = Input.mouseScrollDelta;
            if (scroll.y == 0) return;
            var delta = scroll.y > 0 ? 1 : -1;
            _empowerIndex += delta;
            if (_empowerIndex + delta > empowerData.Count) {
                _empowerIndex = 0;
            } else if (_empowerIndex + delta < 0) {
                _empowerIndex = empowerData.Count - 1;
            }
            var currentData = empowerData[_empowerIndex];
            _currentEmpowerType = currentData.type;
            this.FireEvent(EventType.AugmentChangedEvent, _currentEmpowerType);
            this.FireEvent(EventType.AugmentChargeEvent, currentData.chargeDuration);
            ResetEmpower(currentData);
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

        private void OnInventoryUIEvent(InventoryToggleMsg msg) {
            _canChangeEmpower = !msg.state;
        }

        private bool CanDamage(EnemyBase enemy) => _currentEmpowerType == empowerData.Find(x => enemy.type == x.counterType).type;

        private void UpdateCurrentWeapon(WeaponEntry weapon) {
            _currentWeapon = weapon.reference;
        }

        private IEnumerator EnableEmpowerRoutine(EmpowerType type) {
            _empoweredWeapon = _currentWeapon;
            _currentEmpowerType = type;
            this.FireEvent(EventType.AugmentDrainEvent, duration);
            yield return new WaitForSeconds(duration);
            _empoweredWeapon = null;
            _currentEmpowerType = EmpowerType.None;
        }
    }
}