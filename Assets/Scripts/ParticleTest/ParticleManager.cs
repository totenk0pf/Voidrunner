using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Patterns;

namespace Level
{
    public class ParticleManager : PoolBase<ParticleTest>
    {
        [SerializeField] private ParticleTest particlePrefab;
    }
}