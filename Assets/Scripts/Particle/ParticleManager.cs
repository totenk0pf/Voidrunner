using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using Core.Patterns;
using EventType = Core.Events.EventType;

namespace Particle
{
    [Serializable]
    public struct ParticleInitData
    {
        public Vector3 normal;
        public Vector3 position;
    }
    
    public class ParticleManager : PoolBase<ParticleTest>
    {
        
        [SerializeField] private ParticleTest particlePrefab;
        //private ParticleTest particlePrefab;
        
        [SerializeField] private int initialPoolSize = 10;
        [SerializeField] private int maxPoolSize = 50;

        [Header("Particle Attributes")] 
        [SerializeField] private float duration = 1;
        [SerializeField] private ParticleSystem.MinMaxCurve startLifetime = 1;
        [SerializeField] private bool canLoop = false;
        
        private void Awake()
        {
            EventDispatcher.Instance.AddListener(EventType.SpawnParticleEvent, param => SpawnParticle((ParticleInitData) param));
        }
        
        
        private void Start()
        {
            //Unity's Pool system already created a dynamic array behind the scene with "initialPoolSize" as starting array size,
            //and will resize up to every (initialPoolSize + n * resize#) until it reaches "maxPoolSize"
            InitPool(particlePrefab, initialPoolSize, maxPoolSize);
            
            //There's no need for this initialization of game objects, (This for loop can be get rid of)
            //because Pool.Get() will create new game objects if there's none or not enough
            //just doing this at start so it doesn't have to instantiate a fuck ton at some point in game
            //(like shotgun blast with tons of raycast points to spawn particle could be expensive?, maybe)
            for (var i = 0; i < initialPoolSize; i++)
            {
                var obj = CreateSetup();
                obj.transform.parent = transform;
                Release(obj);
            }
        }

        private void Update()
        {
            TestFireRayCast();
        }
        
        //TODO: this is for testing, move this elsewhere if needed, use event dispatcher.
        private void TestFireRayCast()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    ParticleInitData data = new ParticleInitData() {
                        normal = hit.normal,
                        position = hit.point
                    };
                    
                    EventDispatcher.Instance.FireEvent(EventType.SpawnParticleEvent, data);
                }
            }
        }
        
        private void SpawnParticle(ParticleInitData data)
        {
            var particle = Get();
            particle.Init(data, Release);
        }

       #region Overrides

       protected override ParticleTest CreateSetup()
       {
           //instantiate and set parent to manager
           var obj = Instantiate(particlePrefab, transform, true);

           var particle = obj.GetComponent<ParticleTest>();

           var ps = particle.ParticleSystem;
           ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

           var main = ps.main;
           main.duration = duration;
           main.startLifetime = startLifetime;
           main.loop = canLoop;
           
           return obj;
       }
       
       
       #endregion
    }
}


