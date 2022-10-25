using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventorySystem : MonoBehaviour
{
    private Dictionary<InventoryItemData, InventoryItem> _itemDict;
    public List<InventoryItem> Inventory { get; private set; }

    private void Awake()
    {
        Inventory = new List<InventoryItem>();
        _itemDict = new Dictionary<InventoryItemData, InventoryItem>();
    }

    public void Add(InventoryItemData data)
    {
        if(_itemDict.TryGetValue(data, out InventoryItem value))
        {
            value.AddItem();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(data);//has add item built in
            Inventory.Add(newItem);
            _itemDict.Add(data, newItem);
        }
    }

    public void Remove(InventoryItemData data)
    {
        if (_itemDict.TryGetValue(data, out InventoryItem value))
        {
            value.RemoveItem();

            if (value.ItemCount == 0)
            {
                Inventory.Remove(value);
                _itemDict.Remove(data);
            }
        }
    }

    public InventoryItem Get(InventoryItemData data)
    {
        return _itemDict.TryGetValue(data, out InventoryItem value) ? value : null;
    }
}


[Serializable]
public class InventoryItem
{
    public InventoryItemData Data { get; private set; }
    public int ItemCount { get; private set; }

    public InventoryItem(InventoryItemData source)
    {
        Data = source;
        AddItem();
    }

    public void AddItem()
    {
        ItemCount++;
    }

    public void RemoveItem()
    {
        ItemCount--;
    }
}