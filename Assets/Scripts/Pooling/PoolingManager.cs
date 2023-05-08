using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Logging;
using UnityEngine;
using Core.Patterns;
using Unity.VisualScripting;
using EventType = Core.Events.EventType;

[Serializable]
public class PooledData
{
    public ParticleBase prefab;
    public int initialPoolSize = 10;
    public int maxPoolSize = 50;
}


public class PoolingManager : MonoBehaviour
{
    [SerializeField] private PoolManagerData alwaysLoaded_PoolData;
    [SerializeField] private PoolManagerData sceneBased_PoolData_Particle;

    private Dictionary<EventType, PooledData> _eventToPooledData = new();

    private void Awake()
    {
        _eventToPooledData = _eventToPooledData.AddRange(alwaysLoaded_PoolData.eventToPooledData)
            .AddRange(sceneBased_PoolData_Particle.eventToPooledData);
    
    
        // foreach(var kvp in sceneBased_PoolData_Particle.eventToPooledData){
        //     ParticleManager.Instance.AddListener(kvp.Key, param => SpawnPooledObject(kvp.Key, (PooledObjectCallbackData) param));
        // }
    }
    
    
    private void Start()
    {
        //Init For all note types in lane
        foreach (var kvp in sceneBased_PoolData_Particle.eventToPooledData) {
            SetUpPool(kvp, ParticleManager.Instance.transform);
        }
    }

    private void Update()
    {
       // TestFireRayCast();
    }
    
    //TODO: this is for testing, move this elsewhere if needed, use event dispatcher.
    private void TestFireRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                //ActivateParticleFromPool(hit, EventType.SpawnParticleREDEvent);
                this.FireEvent(EventType.SpawnParticleREDEvent, new ParticleCallbackData(hit.normal, hit.point));
            }
        }
        
        if (Input.GetMouseButtonDown(1)) {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
               // ActivateParticleFromPool(hit, EventType.SpawnParticleGREENEvent);
            }
        }
        
        if (Input.GetMouseButtonDown(2)) {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                //ActivateParticleFromPool(hit, EventType.SpawnParticleBLUEEvent);
            }
        }
    }
    
    
    // private void SpawnPooledObject(EventType eventType, PooledObjectCallbackData data)
    // {
    //     switch (eventType)
    //     {
    //         case EventType.SpawnParticleREDEvent:
    //             SpawnParticle(eventType, data);
    //             break;
    //         case EventType.SpawnParticleGREENEvent:
    //             SpawnParticle(eventType, data);
    //             break;
    //         case EventType.SpawnParticleBLUEEvent:
    //             SpawnParticle(eventType, data);
    //             break;
    //     }
    // }

    private void SetUpPool(KeyValuePair<EventType, PooledData> kvp, Transform parent) 
    {
        GameObject poolGO = new GameObject();
        poolGO.transform.parent = parent;
        poolGO.name = $"Pool {kvp.Value.prefab.name}";
        
        var pool = poolGO.AddComponent<ObjectPool>();
        pool.InitPool(kvp.Value.prefab, parent, kvp.Value.initialPoolSize, kvp.Value.maxPoolSize);
        pool.Init(kvp.Value.prefab, kvp.Key);
        for (var i = 0; i < kvp.Value.initialPoolSize; i++) {
            var obj = pool.CreateSetup();
            obj.transform.SetParent(poolGO.transform, false);
            pool.Release(obj);
        }
    }
    
   #region Overrides
   
   #endregion
}


public static class DictionaryExtension
{
    public static Dictionary<T, S> AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException("Collection is null");
        }

        foreach (var item in collection)
        {
            if(!source.ContainsKey(item.Key)){ 
                source.Add(item.Key, item.Value);
            }
            else
            {
                // handle duplicate key issue here
                NCLogger.Log($"item.Key {item.Key} & item.Value {item.Value} is duplicated", LogLevel.ERROR);
            }  
        }

        return source;
    }
}


