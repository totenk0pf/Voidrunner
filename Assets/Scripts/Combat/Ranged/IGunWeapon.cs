using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGunWeapon
{
    public IEnumerator Fire();
    public IEnumerator Reload();
    
}
