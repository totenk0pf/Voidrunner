using UnityEngine;
using System;
using Items;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Inventory/Item Data")]
[Serializable]
public class ItemData : SerializedScriptableObject
{
    [TitleGroup("Item")]
    public SerializableGuid id;
    public IItemBehaviour behaviour;

    [TitleGroup("Display")]
    public string itemName;
    public string itemDescription;
    public Sprite icon;
    
    [TitleGroup("World item")]
    public Mesh model;
    public Material material;
    
    [TitleGroup("Inventory")]
    public float weight;
    public bool canDrop;
    public bool isInfinite;
}