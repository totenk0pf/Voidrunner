using System;
using Core.Patterns;
using UnityEngine;


public class ParticleManager : Singleton<ParticleManager>, IPoolerCore
{
    private void Awake()
    {
        ;
    }

    
    
    public void GetObject(PooledObjectCallbackData data)
    {
        ;
    }
}
