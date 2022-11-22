using Core.Patterns;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Particle
{
    public class ParticlePool : PoolBase<ParticleBase>
    {
        public EventType particleEventType;
    }
}