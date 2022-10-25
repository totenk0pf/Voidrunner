using UnityEngine;

[CreateAssetMenu(menuName = "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    public ItemID id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;
}

public enum ItemID
{
    ItemA,
    ItemB,
    ItemC
}