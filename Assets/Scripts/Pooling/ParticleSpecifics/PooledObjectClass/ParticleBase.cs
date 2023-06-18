using System;
using System.Collections;
using Core.Logging;
using UnityEngine;


[Serializable]
public class ParticleCallbackData : PooledObjectCallbackData
{
    public Vector3 normal;
    public Transform tempParent;
    public ParticleCallbackData(Vector3 normal, Vector3 position, Transform tempParent) : base(position)
    {
        this.normal = normal;
        this.position = position;
        this.tempParent = tempParent;
    }
    
    public ParticleCallbackData(Vector3 normal, Vector3 position) : base(position)
    {
        this.normal = normal;
        this.position = position;
    }
}
    

public class ParticleBase : PooledObjectBase
{
    private Transform _ogParent;
    [SerializeField] private ParticleSystem _ps;
    public ParticleSystem ParticleSystem {
        get {
            if (!_ps) _ps = GetComponentsInChildren<ParticleSystem>()[0];
            return _ps;
        } 
    }
    
    public ParticleData particleData;


    protected void Awake()
    {
        ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _ogParent = transform.parent;
    }

    protected virtual void Start()
    {
        ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        var main = ParticleSystem.main;
        main.duration = particleData.duration;
        main.loop = particleData.canLoop;
    }

    public override void Init(PooledObjectCallbackData data, Action<PooledObjectBase> killAction)
    {
        _ogParent = transform.parent;
        ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        KillAction = killAction;

        try {
            transform.position = (data as ParticleCallbackData).position;
            transform.up = (data as ParticleCallbackData).normal;

            if ((data as ParticleCallbackData).tempParent != null)
            {
                transform.parent = (data as ParticleCallbackData).tempParent;
            }
        } catch {
            NCLogger.Log($"particle call back data is null", LogLevel.ERROR);
        }
        
    
        ParticleSystem.Play();
        StartCoroutine(RunRoutine());
    }

    public void ForceRelease()
    {
        canRelease = true;
    }
    
    public override IEnumerator RunRoutine()
    {
        while (ParticleSystem.isPlaying)
        {
            //transform.up = UnityEngine.Random.onUnitSphere;
            // NCLogger.Log($"Running");
            yield return null;
            if (canRelease)
            {
                NCLogger.Log($"ForceReleased");
                ParticleSystem.Stop();
                break;
            }
        }

        transform.parent = _ogParent;
        KillAction(this);
        yield return null;
    }
}
