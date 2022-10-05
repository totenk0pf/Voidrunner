using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeleeWeapon
{
    protected abstract void Fire();
    protected abstract void AltFire();
}
