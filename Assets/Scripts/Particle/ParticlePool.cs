using Core.Patterns;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Particle
{
    public class ParticlePool : PoolBase<ParticleBase>
    {
        private EventType _particleEventType;
        public EventType ParticleEventType {
            get => _particleEventType;
        }
        
        private ParticleBase _prefab;

        
        
        public void OnPoolSpawn(ParticleBase prefab, EventType eventType)
        {
            _prefab = prefab;
            _particleEventType = eventType;
        }
        
        public override ParticleBase CreateSetup()
        {
            return Instantiate(_prefab, transform);
        }
    }
}