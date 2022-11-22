using System;
using System.Collections;
using UnityEngine;

namespace Particle
{
    public class ParticleBase : MonoBehaviour, IParticleCore<ParticleBase>
    {
        private ParticleSystem _ps;
        public ParticleSystem ParticleSystem {
            get {
                if (!_ps) _ps = GetComponent<ParticleSystem>();
                return _ps;
            } 
        }

        private Action<ParticleBase> KillAction;
        public ParticleData particleData;
    
    
        protected virtual void Awake()
        {
            var main = ParticleSystem.main;
            main.duration = particleData.duration;
            main.startLifetime = particleData.startLifeTime;
            main.loop = particleData.canLoop;
        }

        public virtual void Init(ParticleInitData data, Action<ParticleBase> killAction)
        {
            ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            KillAction = killAction;
        
            transform.position = data.position;
            transform.up = data.normal;
        
            ParticleSystem.Play();
            StartCoroutine(RunRoutine());
        }
        
        public virtual IEnumerator RunRoutine()
        {
            while (ParticleSystem.isPlaying) {
                yield return null;
            }
        
            KillAction(this);
            yield return null;
        }
    }
}