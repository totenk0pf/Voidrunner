using System;
using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Core.Events;
using Core.Logging;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;

[Serializable]
public class ComboAnimContainer
{
    [ReadOnly] public Transform transform;
    [ReadOnly] public Vector3 direction;
    public float distance;
    public float time;
    public Ease easeType;

    public ComboAnimContainer(Transform transform, Vector3 direction, float distance, float time, Ease easeType) {
        this.transform = transform;
        this.direction = direction;
        this.distance = distance;
        this.time = time;
        this.easeType = easeType;
    }
}

public class CombatAnimationSequenceController : MonoBehaviour
{
    private void Awake() {
        this.AddListener(EventType.RunPlayerComboSequenceEvent, param => PlayComboAnimation((ComboAnimContainer) param));
    }
    
    private void PlayComboAnimation(ComboAnimContainer param)
    {
        if (param.transform == null) {
            NCLogger.Log($"param.transform: {param.transform}", LogLevel.ERROR);
            return;
        }

        if (param.direction == Vector3.zero) {
            NCLogger.Log($"param.direction: {param.direction}", LogLevel.ERROR);
            return;
        }
        
        param.transform.DOMove(param.transform.position + param.direction * param.distance , param.time).SetEase(param.easeType);
    }
}