using System.Collections.Generic;
using UnityEngine;
using Core.Events;
using Sirenix.OdinInspector;
using UI;
using EventType = Core.Events.EventType;

public struct ItemMsg {
    public ItemData data;
    public int count;
}

public class InventorySystem : SerializedMonoBehaviour {
    [ReadOnly] public List<InventoryItem> inventory;
    private InventoryItem _activeItem;
    [SerializeField] private ItemData defaultItem;
    private float _currentWeight;
    [SerializeField] private float maxWeight;
    private bool isUIActive;

    private void Awake() {
        isUIActive = false;
        inventory  = new List<InventoryItem>();
        this.AddListener(EventType.ItemAddEvent, data => Add((ItemMsg) data));
        this.AddListener(EventType.ItemPickEvent, data => Pick((ItemData) data));
        Add(new ItemMsg {
            data = defaultItem,
            count = 1
        });
        _activeItem = inventory.Find(x => x.data == defaultItem);
        this.FireEvent(EventType.InventoryUpdateEvent, new InventoryUpdateMsg {
            currentWeight = _currentWeight,
            maxWeight = maxWeight,
            activeItem = _activeItem,
            itemOnly = false
        });
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (inventory.Count < 1) return;
            UseItem(_activeItem);
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            UpdateUI();
        }
    }

    public void UpdateUI() {
        this.FireEvent(EventType.InventoryToggleEvent, new InventoryToggleMsg {
            state = !isUIActive
        });
        isUIActive = !isUIActive;
    }

    public void Pick(ItemData data) {
        _activeItem = inventory.Find(x => x.data == data);
    }

    public void Add(ItemMsg itemMsg) {
        var data = itemMsg.data;
        var count = itemMsg.count;
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
        this.FireEvent(EventType.ItemRemoveEvent, new ItemMsg {
            data = item.data,
            count = -1
        });
    }
}