using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using DG.Tweening;
using Level;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;
using UnityEngine.Serialization;
using EventType = Core.Events.EventType;

namespace Level {
    [RequireComponent(typeof(BoxCollider))]
    public class ItemDoor : Door {
        [SerializeField] private List<ItemData> requiredItems;

        protected override bool InheritedCheck(Collider other) {
            InventorySystem inv = other.gameObject.GetComponent<InventorySystem>();
            var invItems = inv.inventory.Select(item => item.data).ToList();
            return requiredItems.All(x => invItems.Contains(x));
        }
    }
}