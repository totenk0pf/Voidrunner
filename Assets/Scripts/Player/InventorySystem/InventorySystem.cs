using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Core.Events;
using Items;

public struct PickupMsg {
    public ItemData data;
    public int count;
}

public class InventorySystem : MonoBehaviour {
    public List<InventoryItem> Inventory;
    public float currentWeight;
    public float maxWeight;

    //public InventoryItemData testData;

    private void Awake() {
        Inventory = new List<InventoryItem>();
        EventDispatcher.Instance.AddListener(Core.Events.EventType.OnItemAdd, (data) => Add((PickupMsg) data));
    }

    public void Add(PickupMsg pickupMsg) {
        var data = pickupMsg.data;
        var count = pickupMsg.count;
        if (currentWeight + data.weight * count > maxWeight) return;
        if (ItemManager.Instance.itemList.TryGetValue(data, out InventoryItem value)) {
            value.AddItem();
        } else {
            Debug.Log("add new");
            InventoryItem newItem = new InventoryItem(data); //has add item built in

            Inventory.Add(newItem);
            _itemDict.Add(data, newItem);
        }

        currentWeight += data.weight;
    }

    public void Remove(ItemData data) {
        if (_itemDict.TryGetValue(data, out InventoryItem value)) {
            value.RemoveItem();
            currentWeight -= data.weight;

            if (value.ItemCount == 0) {
                Inventory.Remove(value);
                _itemDict.Remove(data);
            }
        }
    }

    public InventoryItem Get(ItemData data) {
        return _itemDict.TryGetValue(data, out InventoryItem value) ? value : null;
    }

    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Remove(testData);
        }
    }*/
}


[Serializable]
public class InventoryItem {
    public ItemData Data;
    public int ItemCount;
    public float totalWeight;

    public InventoryItem(ItemData source) {
        Data = source;
        AddItem();
    }

    public void AddItem() {
        ItemCount++;
        totalWeight += Data.weight;
    }

    public void RemoveItem() {
        ItemCount--;

        if (ItemCount <= 0) totalWeight = 0;
        else totalWeight -= Data.weight;
    }
}