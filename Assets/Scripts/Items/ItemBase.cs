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

    public virtual void UseItem() 
    { 
    
    }

    public virtual void PickUpItem() 
    { 
    
    }

    public virtual void RemoveItem() 
    { 
    
    }
}
