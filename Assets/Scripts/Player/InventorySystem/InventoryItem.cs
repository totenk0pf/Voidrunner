using System;

[Serializable]
public class InventoryItem {
    public ItemData data;
    public int itemCount;
    public float totalWeight;
    public bool isInfinite;

    public InventoryItem(ItemData source) {
        data = source;
        AddItem();
    }

    public void AddItem() {
        if (isInfinite) return;
        itemCount++;
        totalWeight += data.weight;
    }

    public void RemoveItem() {
        if (isInfinite) return;
        itemCount--;
        totalWeight = itemCount <= 0 ? 0 : totalWeight - data.weight;
    }
}