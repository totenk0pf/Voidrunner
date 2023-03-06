using System;
using System.Collections.Generic;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Combat
{
    [Serializable]
    [CreateAssetMenu(fileName = "SerializedMeleeSequence", menuName = "Player/SerializedMeleeSequenceData", order = 0)]
    public class MeleeSequenceData : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<MeleeOrder, MeleeSequenceAttribute> orderToAttributes;
        public Dictionary<MeleeOrder, MeleeSequenceAttribute> OrderToAttributes => orderToAttributes;

        public bool ValidateColliders() {
            var result = true;
            foreach (var order in orderToAttributes.Keys) {
                var attribute = orderToAttributes[order];
                if (attribute.collider) continue;
                NCLogger.Log($"Collider at {order} missing Ref", LogLevel.ERROR);
                result = false;
            }
            return result;
        }
    }
}