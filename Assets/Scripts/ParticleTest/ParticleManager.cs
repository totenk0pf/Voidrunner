using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using Core.Patterns;
using EventType = Core.Events.EventType;

namespace Level
{
    public class ParticleManager : PoolBase<ParticleTest>
    {
        [SerializeField] private ParticleTest particlePrefab;
        //private ParticleTest particlePrefab;
        
        [SerializeField] private int initialPoolSize = 10;
        [SerializeField] private int maxPoolSize = 50;

        private void Awake()
        {
            EventDispatcher.Instance.AddListener(EventType.SpawnParticleEvent, o => SpawnParticle((GameObject) o));
        }
        
        
        private void Start()
        {
            InitPool(particlePrefab, initialPoolSize, maxPoolSize);
        }

        private void SpawnParticle(GameObject particlePrefab)
        {
            
        }
    }
}