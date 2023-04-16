using System;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;

namespace Combat
{
    [Serializable]
    public class WeaponEntry {
        public KeyCode key;
        public WeaponType type;
        public WeaponBase reference;
    }
    
    [CreateAssetMenu(fileName = "WeaponEntriesData", menuName = "Weapon/WeaponEntriesData", order = 0)]
    public class WeaponEntriesData : ScriptableObject
    {
        public List<WeaponEntry> weaponList;

        public WeaponEntry GetReference(WeaponType typeToHook)
        {
            foreach (var entry in weaponList) {
                if (entry.type == typeToHook) {
                    if(entry.reference != null) return entry;
                    NCLogger.Log($"Cannot Hook Reference - entry {entry.type} has Null Reference", LogLevel.ERROR);
                    return null;
                }
            }
            NCLogger.Log($"Cannot Hook reference - Type Mis-Match", LogLevel.ERROR);
            return null;
        }
        
        public WeaponEntry HookReference(WeaponBase reference, WeaponType typeToHook)
        {
            foreach (var entry in weaponList) {
                if (entry.type == typeToHook) {
                    entry.reference = reference;
                    return entry;
                }
            }
            NCLogger.Log($"Cannot Hook reference - Type Mis-Match", LogLevel.ERROR);
            return null;
        }
    }
}