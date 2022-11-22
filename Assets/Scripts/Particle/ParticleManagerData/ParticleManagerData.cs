using System.Collections.Generic;
using Particle;
using UnityEngine;

namespace Particle
{
    [CreateAssetMenu(fileName = "ParticleManagerData_SceneX", menuName = "Particle Manager Data", order = 0)]
    public class ParticleManagerData : ScriptableObject
    {
        public List<GameObject> particleToSpawnList = new List<GameObject>();
    }
}