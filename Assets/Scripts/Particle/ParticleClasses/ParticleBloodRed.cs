using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle;

namespace Particle
{
    // [RequireComponent(typeof(ParticleSystem))]
    // public class ParticleBloodRed : ParticleBase<ParticleBloodRed>, IParticleCore<ParticleBloodRed>
    // {
    //     public override void Init(ParticleInitData data, Action<ParticleBloodRed> killAction)
    //     {
    //         base.Init(data, killAction);
    //         StartCoroutine(RunRoutine());
    //     }
    //
    //     public override IEnumerator RunRoutine()
    //     {
    //         while (ParticleSystem.isPlaying) {
    //             yield return null;
    //         }
    //     
    //         KillAction(this);
    //         yield return null;
    //     }
    // }
}

