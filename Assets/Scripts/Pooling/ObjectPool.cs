using Core.Events;
using Core.Patterns;
using UnityEngine;
using EventType = Core.Events.EventType;


public class ObjectPool : PoolBase<PooledObjectBase>
{
    private EventType _pooledObjectEventType;
    public EventType PooledObjectEventType {
        get => _pooledObjectEventType;
    }
    
    private PooledObjectBase _prefabRef;

    public virtual void Init(PooledObjectBase prefab, EventType eventType)
    {
        _prefabRef = prefab;
        _pooledObjectEventType = eventType;
        this.AddListener(_pooledObjectEventType, 
            param=> (this.Get()).Init((PooledObjectCallbackData)param, this.Release));
    }

    private void OnDestroy() {
        this.RemoveListener(_pooledObjectEventType, 
                         param=> (this.Get()).Init((PooledObjectCallbackData)param, this.Release));
    }

    public override PooledObjectBase CreateSetup()
    {
        return Instantiate(_prefabRef, transform);
    }

    public void PrewarmAllParticle(int cycle)
    {
        for (var i = 0; i < cycle; i++)
        {
            this.Get().Init( new ParticleCallbackData(transform.up, transform.position), this.Release);
        }
    }
}
