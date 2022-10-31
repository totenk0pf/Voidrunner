using UnityEngine;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [TitleGroup("Item")]
    public SerializableGuid id;

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
}