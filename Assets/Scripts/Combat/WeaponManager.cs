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
<<<<<<< HEAD
        [Serializable]
        public class WeaponEntry {
            [SerializeField] private KeyCode keymap;
            [SerializeField] private WeaponType type;
            [SerializeField] private WeaponBase reference;

            public KeyCode KeyMap => keymap;
            public WeaponType Type => type;
            public WeaponBase Reference => reference;
        }
        
=======
>>>>>>> c9f1fe8cad10044d48d8fb74e790012081e956ad
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
<<<<<<< HEAD
                if (Input.GetKeyDown(item.KeyMap)) {
                    OnWeaponSwap(item.Type);
=======
                if (Input.GetKeyDown(item.keymap)) {
                    OnWeaponSwap(item);
>>>>>>> c9f1fe8cad10044d48d8fb74e790012081e956ad
                }
            }
        }

<<<<<<< HEAD
        private void OnWeaponSwap(WeaponType weapon)
        {
            WeaponEntry entry = null;
            foreach (var item in weaponList) {
                if (item.Type != weapon) continue;
                
                item.Reference.canAttack = true;
                entry = item;
                break;
=======
        private void OnWeaponSwap(WeaponEntry weapon) {
            foreach (var item in weaponList) {
                item.reference.canAttack = item.type == weapon.type;
>>>>>>> c9f1fe8cad10044d48d8fb74e790012081e956ad
            }
            currentWeapon = weapon;
            this.FireEvent(EventType.WeaponChangedEvent, entry);
        }

        private void OnValidate() {
            var entry = weaponList.Find(x => x.type == currentType);
            currentWeapon = entry;
        }
    }
}