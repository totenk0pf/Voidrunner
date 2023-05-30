using System.Collections.Generic;
using Core.Debug;
using UnityEngine;
using Core.Events;
using Sirenix.OdinInspector;
using UI;
using EventType = Core.Events.EventType;

public struct ItemMsg {
    public ItemData data;
}

public class InventorySystem : SerializedMonoBehaviour {
    [ReadOnly] public List<InventoryItem> inventory;
    
    [TitleGroup("Items")]
    private InventoryItem _activeItem;
    [SerializeField] private ItemData defaultItem;
    
    [TitleGroup("Weight")]
    private float _currentWeight;
    [SerializeField] private float maxWeight;
    [SerializeField] private float weightPerLevel;
    [SerializeField] private float weightModifier;
    [SerializeField] private float defaultWeight;
    
    private bool _isUIActive;
    private bool _canUseItem;

    private void Awake() {
        _isUIActive = false;
        _canUseItem = true;
        maxWeight   = defaultWeight;
        inventory   = new List<InventoryItem>();
        this.AddListener(EventType.ItemAddEvent, data => Add((ItemMsg) data));
        this.AddListener(EventType.ItemPickEvent, data => Pick((ItemMsg) data));
        this.AddListener(EventType.UpdateInventoryData, specAmt => SetMaxWeight((int) specAmt * weightPerLevel * weightModifier));
        Add(new ItemMsg {
            data  = defaultItem
        });
        _activeItem = inventory.Find(x => x.data == defaultItem);
        this.FireEvent(EventType.InventoryUpdateEvent, new InventoryUpdateMsg {
            currentWeight = _currentWeight,
            maxWeight     = maxWeight,
            activeItem    = _activeItem,
            itemOnly      = false
        });
        this.AddListener(EventType.InventoryToggleEvent, msg => UpdateUI((InventoryToggleMsg) msg));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (!_canUseItem) return;
            UseItem(_activeItem);
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            this.FireEvent(EventType.InventoryToggleEvent, new InventoryToggleMsg {
                state = !_isUIActive
            });
            this.FireEvent(EventType.InventoryUpdateEvent, new InventoryUpdateMsg {
                currentWeight = _currentWeight,
                maxWeight     = maxWeight,
                activeItem    = _activeItem,
                itemOnly      = false
            });
        }
#if UNITY_EDITOR
        DebugGUI.Instance.UpdateText(nameof(InventorySystem),
            "\nInventory\n" +
            $"Weight: {_currentWeight}\n" + 
            $"Max weight: {maxWeight}\n" +
            $"Weight mod: {weightModifier}\n"
        );
#endif
    }

    public void UpdateUI(InventoryToggleMsg msg) {
        _isUIActive = msg.state;
        _canUseItem = !msg.state;
    }

    public void Pick(ItemMsg msg) {
        _activeItem = inventory.Find(x => x.data == msg.data);
    }

    public void Add(ItemMsg itemMsg) {
        var data = itemMsg.data;
        if (_currentWeight + data.weight > maxWeight) return;
        var item = inventory.Find(x => x.data == data);
        if (item != null) {
            item.AddItem();
        } else {
            var newItem = new InventoryItem(data);
            inventory.Add(newItem);
        }
        _currentWeight += data.weight;
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
        if (item.itemCount <= 0) return;
        item.data.behaviour.Execute(this);
        if (item.data.isInfinite) return;
        Remove(item.data);
        this.FireEvent(EventType.ItemRemoveEvent, new ItemMsg {
            data  = item.data
        });
        this.FireEvent(EventType.InventoryHUDEvent, new InventoryHUDMsg {
            count = _activeItem.itemCount,
            countOnly = true
        });
    }

    public void SetMaxWeight(float amount) {
        maxWeight = amount;
    }
    
    public void IncreaseMaxWeight(float amount) {
        maxWeight += weightModifier + amount;
    }
}