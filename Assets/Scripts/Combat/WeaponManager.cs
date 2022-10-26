using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Patterns;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat {
    public enum WeaponType {
        Melee,
        Ranged
    }
    
    public class WeaponManager : MonoBehaviour {
        [Serializable]
        public struct WeaponEntry {
            public KeyCode keymap;
            public WeaponType type;
            public WeaponBase reference;
        }
        
        [SerializeField] private List<WeaponEntry> weaponList;
        [SerializeField] private WeaponType currentWeapon;

        private void Awake() {
            OnWeaponSwap(currentWeapon);
            
        }

        private void Update() {
            foreach (var item in weaponList) {
                if (Input.GetKeyDown(item.keymap)) {
                    OnWeaponSwap(item.type);
                }
            }
        }

        private void OnWeaponSwap(WeaponType weapon)
        {
            WeaponBase currWeapon = null;//declaring
            foreach (var item in weaponList) {
                item.reference.gameObject.SetActive(item.type == weapon);
                if (item.type == weapon) currWeapon = item.reference;//get current weapon base
            }
            currentWeapon = weapon;
            EventDispatcher.Instance.FireEvent(EventType.WeaponChangedEvent, currentWeapon);
            
            EventDispatcher.Instance.FireEvent(EventType.CurrentWeaponChangeEvent, currWeapon);//fire event
        }

        // private WeaponBase GetCurrentWeapon()
        // {
        //     return (from entry in weaponList where entry.type == currentWeapon select entry.reference).FirstOrDefault();
        // }
    }
}