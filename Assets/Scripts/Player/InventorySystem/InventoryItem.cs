using System;

[Serializable]
public class InventoryItem {
    public ItemData data;
    public int itemCount;
    public float totalWeight;

    public InventoryItem(ItemData source) {
        data = source;
        AddItem();
    }

    public void AddItem() {
        if (data.isInfinite) return;
        itemCount++;
        totalWeight += data.weight;
    }

    public void RemoveItem() {
        if (data.isInfinite) return;
        itemCount--;
        totalWeight = itemCount <= 0 ? 0 : totalWeight - data.weight;
    }
}