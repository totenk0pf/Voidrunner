using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Unity.VisualScripting;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat
{
    public class WeaponEmpowerment : MonoBehaviour
    {
        //KeyCode to activate empower
        [Header("Empower Keys")]
        [SerializeField] private KeyCode keyFire;
        [SerializeField] private KeyCode keyElectric;
        [SerializeField] private KeyCode keyAcid;
        private EmpowerType _currentEmpowerType = EmpowerType.None;

        
        
        [Header("Empower Attributes")]
        private WeaponBase _currentWeapon; //current weapon that's equipped != not necessarily empowered weapon
        private WeaponBase _empoweredWeapon;
        [SerializeField] private float duration;//Duration for empowerment

        [Header("Fire Attributes")] 
        [SerializeField] private List<string> fleshTags;
        [SerializeField] private float fireDamage; //extra damage against flesh

        [Header("Electric Attributes")] 
        [SerializeField] private List<string> boneTags;
        [SerializeField] private float stunDuration; //stun duration against Bone

        [Header("Acid Attributes")] 
        [SerializeField] private List<string> armorTags;
        [SerializeField] private float acidDamagePerInterval;
        [SerializeField] private float intervalNum;
        [SerializeField] private float intervalDuration;

        private void Awake()
        {
            EventDispatcher.Instance.AddListener(EventType.CurrentWeaponChangeEvent, param => UpdateCurrentWeapon((WeaponBase) param));
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

            switch (_currentEmpowerType)
            {
                case EmpowerType.None:
                    return;
                case EmpowerType.Fire:
                    DealFireEmpowerDamage(enemy);
                    break;
                case EmpowerType.Electric:
                    DealStunEmpowerDamage(enemy);
                    break;
                case EmpowerType.Acid:
                    DealAcidEmpowerDamage(enemy);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private bool DealFireEmpowerDamage(EnemyBase enemy)
        {
            var canDamage = fleshTags.Any(tag => enemy.gameObject.CompareTag(tag));
            if (!canDamage) return false;
            
            enemy.TakeDamage(fireDamage);
            return true;
        }

        private bool DealStunEmpowerDamage(EnemyBase enemy)
        {
            var canDamage = boneTags.Any(tag => enemy.gameObject.CompareTag(tag));
            if (!canDamage) return false;

            StartCoroutine(StunRoutine(enemy));
            return true;
        }

        private bool DealAcidEmpowerDamage(EnemyBase enemy)
        {
            var canDamage = armorTags.Any(tag => enemy.gameObject.CompareTag(tag));
            if (!canDamage) return false;

            StartCoroutine(DOTRoutine(enemy));
            return true;
        }





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
        
        
        private void UpdateCurrentWeapon(WeaponBase weapon)
        {
            _currentWeapon = weapon;
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

            if (Input.GetKeyDown(keyFire))
            {
                StartCoroutine(EnableEmpowerRoutine(EmpowerType.Fire));
            }
            if (Input.GetKeyDown(keyElectric))
            {
                StartCoroutine(EnableEmpowerRoutine(EmpowerType.Electric));
            }
            if (Input.GetKeyDown(keyAcid))
            {
                StartCoroutine(EnableEmpowerRoutine(EmpowerType.Acid));
            }
        }



        // public enum EnemyType
        // {
        //     Walker,
        //     Crawler,
        //     Juggernaut
        // }

        public enum EmpowerType
        {
            None,
            Fire,       //bonus dmg to walker
            Electric,   //stun crawler
            Acid        //DoT juggernaut
        }
    }
}