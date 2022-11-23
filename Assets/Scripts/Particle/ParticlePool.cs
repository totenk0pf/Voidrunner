using Core.Patterns;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Particle
{
    public class ParticlePool : PoolBase<ParticleBase>
    {
        public EventType particleEventType;
        public ParticleBase prefab;
        
        public override ParticleBase CreateSetup()
        {
            return Instantiate(prefab, transform);
        }
    }
}