using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat
{
    public class WeaponEmpowerment : MonoBehaviour
    {
        public enum EmpowerType
        {
            None,
            Fire,       //bonus dmg to walker
            Electric,   //stun crawler
            Acid        //DoT juggernaut
        }

        [TitleGroup("Empower settings")]
        private EmpowerType _currentEmpowerType = EmpowerType.None;
        private WeaponBase _currentWeapon; //current weapon that's equipped != not necessarily empowered weapon
        [SerializeField] private Dictionary<EmpowerType, KeyCode> empowerKeys = new();
        [SerializeField] private Dictionary<EnemyType, EmpowerType> counterType = new();
        private WeaponBase _empoweredWeapon;
        [SerializeField] private float duration; //Duration for empowerment

        [TitleGroup("Fire")]
        [SerializeField] private float fireDamage; //extra damage against flesh

        [TitleGroup("Electric")]
        [SerializeField] private float stunDuration; //stun duration against Bone

        [TitleGroup("Acid")]
        [SerializeField] private float acidDamagePerInterval;
        [SerializeField] private float intervalNum;
        [SerializeField] private float intervalDuration;

        private void Awake()
        {
            EventDispatcher.Instance.AddListener(EventType.WeaponChangedEvent, param => UpdateCurrentWeapon((WeaponEntry) param));
            EventDispatcher.Instance.AddListener(EventType.DamageEnemyEvent, enemy => DealEmpowerDamage((EnemyBase) enemy));
        }

        private void Update()
        {
            UpdateEmpowerStatus();
        }

        
        private void DealEmpowerDamage(EnemyBase enemy)
        {
            //only deal empower damage if current weapon is empowered
            if (_currentWeapon != _empoweredWeapon) return;
            if (!CanDamage(enemy)) return;
            switch (_currentEmpowerType)
            {
                case EmpowerType.None:
                    return;
                case EmpowerType.Fire:
                    enemy.TakeDamage(fireDamage);
                    break;
                case EmpowerType.Electric:
                    StartCoroutine(StunRoutine(enemy));
                    break;
                case EmpowerType.Acid:
                    StartCoroutine(DOTRoutine(enemy));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool CanDamage(EnemyBase enemy) => _currentEmpowerType == counterType[enemy.type];

        private IEnumerator StunRoutine(EnemyBase enemy)
        {
            //TODO: add stun method for enemy here
            //enemy.ToggleStun(true);
            yield return new WaitForSeconds(stunDuration);
            //enemy.ToggleStun(false);
            
            //TODO: Better yet, move the entire stun routine to enemy base or child bone enemy class
            //TODO: so that routine stays local to that enemy.
            //TODO: Right now, it stays like this for placeholder purposes and clear readability
        }

        private IEnumerator DOTRoutine(EnemyBase enemy)
        {
            //TODO: would be preferable if this entire routine moves to the local enemy class
            for (var i = 0; i < intervalNum; i++)
            {
                enemy.TakeDamage(acidDamagePerInterval);
                yield return new WaitForSeconds(intervalDuration);
            }

            yield return null;
        }

        private void UpdateCurrentWeapon(WeaponEntry weapon)
        {
            _currentWeapon = weapon.reference;
        }
        
        private IEnumerator EnableEmpowerRoutine(EmpowerType type)
        {
            _empoweredWeapon = _currentWeapon;
            _currentEmpowerType = type;
            
            yield return new WaitForSeconds(duration);
            
            _empoweredWeapon = null;
            _currentEmpowerType = EmpowerType.None;
        }

        private void UpdateEmpowerStatus()
        {
            if (_currentEmpowerType != EmpowerType.None) return;
            foreach (var item in empowerKeys) {
                if (Input.GetKeyDown(item.Value)) {
                    StartCoroutine(EnableEmpowerRoutine(item.Key));
                }
            }
        }

    }
}