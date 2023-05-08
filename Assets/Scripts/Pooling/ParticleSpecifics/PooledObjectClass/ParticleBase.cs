using System;
using System.Collections;
using Core.Logging;
using UnityEngine;


[Serializable]
public class ParticleCallbackData : PooledObjectCallbackData
{
    public Vector3 normal;

    public ParticleCallbackData(Vector3 normal, Vector3 position) {
        this.normal = normal;
        this.position = position;
    }
}
    

public class ParticleBase : PooledObjectBase
{
    private ParticleSystem _ps;
    public ParticleSystem ParticleSystem {
        get {
            if (!_ps) _ps = GetComponent<ParticleSystem>();
            return _ps;
        } 
    }
    
    public ParticleData particleData;


    protected virtual void Start()
    {
        ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        var main = ParticleSystem.main;
        main.duration = particleData.duration;
        main.startLifetime = particleData.startLifeTime;
        main.loop = particleData.canLoop;
    }

    public override void Init(PooledObjectCallbackData data, Action<PooledObjectBase> killAction)
    {
        ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        KillAction = killAction;

        try {
            transform.position = (data as ParticleCallbackData).position;
            transform.up = (data as ParticleCallbackData).normal;
        } catch {
            NCLogger.Log($"particle call back data is null", LogLevel.ERROR);
        }
        
    
        ParticleSystem.Play();
        StartCoroutine(RunRoutine());
    }
    
    public virtual IEnumerator RunRoutine()
    {
        while (ParticleSystem.isPlaying) {
            yield return null;
        }
    
        KillAction(this);
        yield return null;
    }
}
