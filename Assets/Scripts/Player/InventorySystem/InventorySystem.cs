using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Core.Events;
using Sirenix.OdinInspector;

public struct PickupMsg {
    public ItemData data;
    public int count;
}

public class InventorySystem : MonoBehaviour {
    [ReadOnly] public List<InventoryItem> inventory;
    private InventoryItem _activeItem;
    private float _currentWeight;
    [SerializeField] private float maxWeight;

    private void Awake() {
        inventory = new List<InventoryItem>();
        this.AddListener(Core.Events.EventType.OnItemAdd, (data) => Add((PickupMsg) data));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (inventory.Count < 1) return;
            _activeItem = inventory[0];
            UseItem(_activeItem);
        }
    }

    public void Add(PickupMsg pickupMsg) {
        var data = pickupMsg.data;
        var count = pickupMsg.count;
        if (_currentWeight + data.weight * count > maxWeight) return;
        var item = inventory.Find(x => x.data == data);
        if (item != null) {
            item.AddItem();
        } else {
            var newItem = new InventoryItem(data); 
            inventory.Add(newItem);
            _currentWeight += data.weight;
        }
    }

    public void Remove(ItemData data) {
        var item = Get(data);
        if (item == null) return;
        item.RemoveItem();
        _currentWeight -= data.weight;
        if (item.itemCount == 0) {
            inventory.Remove(item);
        }
    }

    public InventoryItem Get(ItemData data) {
        return inventory.Find(x => x.data == data);
    }

    private void UseItem(InventoryItem item) {
        item.data.behaviour.Execute(this);
        Remove(item.data);
    }
}