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
        public class WeaponEntry {
            [SerializeField] private KeyCode keymap;
            [SerializeField] private WeaponType type;
            [SerializeField] private WeaponBase reference;

            public KeyCode KeyMap => keymap;
            public WeaponType Type => type;
            public WeaponBase Reference => reference;
        }
        
        [SerializeField] private List<WeaponEntry> weaponList;
        [SerializeField] private WeaponType currentWeapon;
        private bool canSwap = true;

        private void Awake() {
            OnWeaponSwap(currentWeapon);
            this.AddListener(EventType.WeaponFiredEvent, param => canSwap = false);
            this.AddListener(EventType.WeaponRechargedEvent, param => canSwap = true);
        }

        private void Update() {
            if (!canSwap) return;
            foreach (var item in weaponList) {
                if (Input.GetKeyDown(item.KeyMap)) {
                    OnWeaponSwap(item.Type);
                }
            }
        }

        private void OnWeaponSwap(WeaponType weapon)
        {
            WeaponEntry entry = null;
            foreach (var item in weaponList) {
                if (item.Type != weapon) continue;
                
                item.Reference.canAttack = true;
                entry = item;
                break;
            }
            currentWeapon = weapon;
            this.FireEvent(EventType.WeaponChangedEvent, entry);
        }
    }
}