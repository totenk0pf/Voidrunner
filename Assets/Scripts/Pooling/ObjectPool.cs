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
    
    private PooledObjectBase _prefab;

    public virtual void Init(PooledObjectBase prefab, EventType eventType)
    {
        _prefab = prefab;
        _pooledObjectEventType = eventType;
        this.AddListener(_pooledObjectEventType, 
            param=> (this.Get()).Init((PooledObjectCallbackData)param, this.Release));
    }
    
    public override PooledObjectBase CreateSetup()
    {
        return Instantiate(_prefab, transform);
    }
}
