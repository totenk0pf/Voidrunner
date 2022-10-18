using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItems 
{
    public void OnUse();
    public void OnPickup();
    public void OnRemove();
}
