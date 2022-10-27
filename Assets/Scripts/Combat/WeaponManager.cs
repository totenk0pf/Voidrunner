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
    
    [Serializable]
    public struct WeaponEntry {
        public KeyCode keymap;
        public WeaponType type;
        public WeaponBase reference;
    }
    
    public class WeaponManager : MonoBehaviour {
        [SerializeField] private List<WeaponEntry> weaponList;
        [SerializeField] private WeaponEntry currentWeapon;
        [SerializeField] private WeaponType currentType;
        private bool canSwap = true;

        private void Awake() {
            OnWeaponSwap(currentWeapon);
            this.AddListener(EventType.WeaponFiredEvent, param => canSwap = false);
            this.AddListener(EventType.WeaponRechargedEvent, param => canSwap = true);
        }

        private void Update() {
            if (!canSwap) return;
            foreach (var item in weaponList) {
                if (Input.GetKeyDown(item.keymap)) {
                    OnWeaponSwap(item);
                }
            }
        }

        private void OnWeaponSwap(WeaponEntry weapon) {
            foreach (var item in weaponList) {
                item.reference.canAttack = item.type == weapon.type;
            }
            currentWeapon = weapon;
            this.FireEvent(EventType.WeaponChangedEvent, currentWeapon);
        }

        private void OnValidate() {
            var entry = weaponList.Find(x => x.type == currentType);
            currentWeapon = entry;
        }
    }
}