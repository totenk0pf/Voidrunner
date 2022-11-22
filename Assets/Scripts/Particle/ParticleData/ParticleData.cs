using UnityEngine;

namespace Particle
{
    [CreateAssetMenu(fileName = "ParticleData", menuName = "Particle Data", order = 0)]
    public class ParticleData : ScriptableObject
    {
        public float duration = 1;
        public ParticleSystem.MinMaxCurve startLifeTime = 1;
        public bool canLoop = false;
    }
}