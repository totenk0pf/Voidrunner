using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using UnityEngine;
using Core.Patterns;
using Unity.VisualScripting;
using EventType = Core.Events.EventType;

namespace Particle
{
    [Serializable]
    public struct ParticleInitData
    {
        public Vector3 normal;
        public Vector3 position;
    }

    public struct ParticleType
    {
        public ParticleInitData InitData;
        public ParticlePool pool;
    }
    
    public class ParticleManager : MonoBehaviour
    {
        [SerializeField] private ParticleManagerData alwaysLoadedParticleData;
        [SerializeField] private ParticleManagerData sceneBasedParticleData;

        //[SerializeField] private ParticlePool poolPrefab;
        
        private List<ParticleEntity> _particleList = new List<ParticleEntity>();
        private List<ParticlePool> _poolList = new List<ParticlePool>();

        private void Awake()
        {
            _particleList = _particleList.Concat(alwaysLoadedParticleData.particleList)
                .Concat(sceneBasedParticleData.particleList)
                .ToList();

            EventDispatcher.Instance.AddListener(EventType.SpawnParticleREDEvent, param => SpawnParticle((ParticleType) param));
            EventDispatcher.Instance.AddListener(EventType.SpawnParticleGREENEvent, param => SpawnParticle((ParticleType) param));
            EventDispatcher.Instance.AddListener(EventType.SpawnParticleBLUEEvent, param => SpawnParticle((ParticleType) param));
        }
        
        
        private void Start()
        {
            //Unity's Pool system already created a dynamic array behind the scene with "initialPoolSize" as starting array size,
            //and will resize up to every (initialPoolSize + n * resize#) until it reaches "maxPoolSize"
            //InitPool(particlePrefab, initialPoolSize, maxPoolSize);
            
            //There's no need for this initialization of game objects, (This for loop can be get rid of)
            //because Pool.Get() will create new game objects if there's none or not enough
            //just doing this at start so it doesn't have to instantiate a fuck ton at some point in game
            //(like shotgun blast with tons of raycast points to spawn particle could be expensive?, maybe)
            // for (var i = 0; i < initialPoolSize; i++)
            // {
            //     var obj = CreateSetup();
            //     Release(obj);
            // }

            foreach (var entity in _particleList)
            {
                GameObject poolGO = new GameObject();
                poolGO.transform.parent = transform;
                poolGO.name = $"Pool {entity.prefab.name}";
                //poolGO.tag = entity.tagName
                var pool = poolGO.AddComponent<ParticlePool>();
                
                pool.particleEventType = entity.particleEventType;
                _poolList.Add(pool);
                
                pool.InitPool(entity.prefab, poolGO.transform, entity.initialPoolSize, entity.maxPoolSize);
                for (var i = 0; i < entity.initialPoolSize; i++) {
                    var obj = pool.CreateSetup();
                    obj.transform.parent = poolGO.transform;
                    pool.Release(obj);
                    
                }
                
            }
        }

        private void Update()
        {
            TestFireRayCast();
        }
        
        //TODO: this is for testing, move this elsewhere if needed, use event dispatcher.
        private void TestFireRayCast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    var type = GetParticlePool(hit, EventType.SpawnParticleREDEvent);
                    EventDispatcher.Instance.FireEvent(EventType.SpawnParticleREDEvent, type);
                }
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    var type = GetParticlePool(hit, EventType.SpawnParticleGREENEvent);
                    EventDispatcher.Instance.FireEvent(EventType.SpawnParticleGREENEvent, type);
                }
            }
            
            if (Input.GetMouseButtonDown(2))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    var type = GetParticlePool(hit, EventType.SpawnParticleBLUEEvent);
                    EventDispatcher.Instance.FireEvent(EventType.SpawnParticleBLUEEvent, type);
                }
            }
        }

        public ParticleType GetParticlePool(RaycastHit hit, EventType type)
        {
            ParticleInitData data = new ParticleInitData() {
                normal = hit.normal,
                position = hit.point
            };
            
            ParticlePool particlePool = new ParticlePool();
            foreach (var pool in _poolList.Where(pool => pool.particleEventType == type)) {
                particlePool = pool;
            }
            
            ParticleType particleType = new ParticleType() {
                InitData = data,
                pool = particlePool
            };

            return particleType;
        }
        
        private void SpawnParticle(ParticleType data)
        {
            var particle = data.pool.Get();
            particle.Init(data.InitData, data.pool.Release);
        }

       #region Overrides

       // protected override ParticleBloodRed CreateSetup()
       // {
       //     //instantiate and set parent to manager
       //     var obj = Instantiate(particlePrefab, transform, true);
       //
       //     var particle = obj.GetComponent<ParticleBloodRed>();
       //
       //     var ps = particle.ParticleSystem;
       //     ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
       //
       //     var main = ps.main;
       //     main.duration = duration;
       //     main.startLifetime = startLifetime;
       //     main.loop = canLoop;
       //     
       //     return obj;
       // }
       
       #endregion
    }
}


