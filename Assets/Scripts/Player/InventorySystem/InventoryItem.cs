using System;

[Serializable]
public class InventoryItem {
    public ItemData data;
    public int itemCount;
    public float totalWeight;

    public InventoryItem(ItemData source, int count = 1) {
        data      = source;
        itemCount = count;
    }

    public void AddItem() {
        if (data.isInfinite) return;
        itemCount++;
        totalWeight += data.weight;
    }

    public void RemoveItem() {
        if (data.isInfinite) return;
        if (itemCount <= 0) return;
        itemCount--;
        totalWeight -= data.weight;
    }
}