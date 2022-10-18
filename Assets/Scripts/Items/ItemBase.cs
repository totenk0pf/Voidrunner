using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour,IItems
{
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
    }

    
}
