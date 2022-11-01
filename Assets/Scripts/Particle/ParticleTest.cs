using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleTest : MonoBehaviour
{
    private ParticleSystem _ps;
    public ParticleSystem ParticleSystem
    {
        get
        {
            if (!_ps) _ps = GetComponent<ParticleSystem>();
            return _ps;
        } 
    }

    private Action<ParticleTest> _killAction;

    public void Init(ParticleInitData data, Action<ParticleTest> killAction)
    {
        _killAction = killAction;
        
        transform.position = data.position;
        transform.up = data.normal;
        
        ParticleSystem.Play();
        StartCoroutine(RunRoutine());
    }

    private IEnumerator RunRoutine()
    {
        while (ParticleSystem.isPlaying) {
            yield return null;
        }
        
        _killAction(this);
        yield return null;
    }
}
