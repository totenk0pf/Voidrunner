using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Events;

public class ItemBase : MonoBehaviour,IItems
{
    public InventoryItemData itemRef;

    public void OnPickup()
    {
        PickUpItem();
    }

    public void OnRemove()
    {
        RemoveItem();
    }

    public void OnUse()
    {
        UseItem();
    }

    public virtual void UseItem() { }

    public virtual void PickUpItem() 
    { 
        //call duong add item function
    }

    public virtual void RemoveItem() 
    {
        //call duong remove item function
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnPickup();
        if (collision.gameObject.CompareTag("Player")) 
        {
            EventDispatcher.Instance.FireEvent(Core.Events.EventType.OnItemAdd, itemRef);
        }
    }

    
}
