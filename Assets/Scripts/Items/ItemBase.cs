using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Events;

public class ItemBase : MonoBehaviour,IItems
{
    public ItemData itemRef;

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
        gameObject.SetActive(false);
    }

    public virtual void RemoveItem() 
    {
        //call duong remove item function
        gameObject.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player")) 
        {
            OnPickup();
            EventDispatcher.Instance.FireEvent(Core.Events.EventType.OnItemAdd, itemRef);
        }
    }

    
}
