using System;
using System.Collections.Generic;
using Particle;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Particle
{
    [Serializable]
    public class ParticleEntity
    {
        public string tagName;
        public ParticleBase prefab;
        public int initialPoolSize = 10;
        public int maxPoolSize = 50;
        public EventType particleEventType;
    }

    [CreateAssetMenu(fileName = "ParticleManagerData_SceneX", menuName = "Particle Manager Data", order = 0)]
    public class ParticleManagerData : ScriptableObject
    {
        public List<ParticleEntity> particleList = new List<ParticleEntity>();
    }
}