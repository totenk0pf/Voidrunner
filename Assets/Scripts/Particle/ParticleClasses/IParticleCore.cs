using System;
using System.Collections;
using System.Collections.Generic;

namespace Particle
{
    public interface IParticleCore<out T>
    {
        void Init(ParticleInitData data, Action<T> killAction);
        IEnumerator RunRoutine();
    }
}