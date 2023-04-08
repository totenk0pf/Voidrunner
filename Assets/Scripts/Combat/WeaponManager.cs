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

    public class WeaponManager : MonoBehaviour
    {

        [Serializable]
        public class WeaponEntry
        {
            [SerializeField] private KeyCode keymap;
            [SerializeField] private WeaponType type;
            [SerializeField] private WeaponBase reference;

            public KeyCode KeyMap => keymap;
            public WeaponType Type => type;
            public WeaponBase Reference => reference;
        }

        [SerializeField] private List<WeaponEntry> weaponList;
        [SerializeField] private WeaponEntry currentWeapon;
        [SerializeField] private WeaponType currentType;
        private bool canSwap = true;

        private void Awake()
        {
            this.AddListener(EventType.WeaponFiredEvent, param => canSwap = false);
            this.AddListener(EventType.WeaponRechargedEvent, param => canSwap = true);
            OnWeaponSwap(currentWeapon);
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (!canSwap) return;
            foreach (var item in weaponList)
            {
                if (Input.GetKeyDown(item.KeyMap))
                {
                    OnWeaponSwap(item);
                }
            }
        }


        // private void OnWeaponSwap(WeaponType weapon)
        // {
        //     WeaponEntry entry = null;
        //     foreach (var item in weaponList)
        //     {
        //         if (item.Type != weapon) continue;
        //
        //         item.Reference.canAttack = true;
        //         entry = item;
        //         break;
        //     }
        // }

        private void OnWeaponSwap(WeaponEntry weapon) {
            foreach (var item in weaponList) {
                item.Reference.canAttack = item.Type == weapon.Type;
            }

            currentWeapon = weapon;
            currentType = currentWeapon.Type;
            this.FireEvent(EventType.WeaponChangedEvent, currentWeapon);
        }

        private void OnValidate()
        {
            var entry = weaponList.Find(x => x.Type == currentType);
            currentWeapon = entry;
        }
    }
}