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

            foreach(var particle in _particleList){
                 EventDispatcher.Instance.AddListener(particle.particleEventType, param => SpawnParticle((ParticleType) param));
            }
        }
        
        
        private void Start()
        {
            
            foreach (var entity in _particleList)
            {
                GameObject poolGO = new GameObject();
                poolGO.transform.parent = transform;
                poolGO.name = $"Pool {entity.prefab.name}";
                //poolGO.tag = entity.tagName
                var pool = poolGO.AddComponent<ParticlePool>();
                
                pool.particleEventType = entity.particleEventType;
                pool.prefab = entity.prefab;
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
            
            if (Input.GetMouseButtonDown(0)) {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                    ActivateParticleFromPool(hit, EventType.SpawnParticleREDEvent);
                }
            }
            
            if (Input.GetMouseButtonDown(1)) {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                    ActivateParticleFromPool(hit, EventType.SpawnParticleGREENEvent);
                }
            }
            
            if (Input.GetMouseButtonDown(2)) {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                    ActivateParticleFromPool(hit, EventType.SpawnParticleBLUEEvent);
                }
            }
        }

        public void ActivateParticleFromPool(RaycastHit hit, EventType type)
        {
            ParticleInitData data = new ParticleInitData() {
                normal = hit.normal,
                position = hit.point
            };
            
            ParticlePool particlePool = new ParticlePool();
            foreach (var pool in _poolList.Where(pool => pool.particleEventType == type)) {
                particlePool = pool;
            }

            if (!particlePool) return;
            
            ParticleType particleType = new ParticleType() {
                InitData = data,
                pool = particlePool
            };
            
            EventDispatcher.Instance.FireEvent(type, particleType);
        }
        
        private void SpawnParticle(ParticleType data)
        {
            var particle = data.pool.Get();
            particle.Init(data.InitData, data.pool.Release);
        }

       #region Overrides
       
       #endregion
    }
}


