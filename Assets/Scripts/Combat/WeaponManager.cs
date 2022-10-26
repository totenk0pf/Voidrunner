using System;
using System.Collections.Generic;
using Core.Events;
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

        private void OnWeaponSwap(WeaponType weapon) {
            foreach (var item in weaponList) {
                item.reference.canAttack = item.type == weapon;
            }
            currentWeapon = weapon;
            EventDispatcher.Instance.FireEvent(EventType.WeaponChangedEvent, currentWeapon);
        }
    }
}