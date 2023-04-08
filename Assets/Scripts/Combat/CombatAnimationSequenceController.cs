using System;
using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Core.Events;
using Core.Logging;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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
    private bool _canPlay = true;
    private TweenerCore<Vector3, Vector3, VectorOptions> _tween;
    private Transform _tweenObj;
    
    private void Awake() {
        this.AddListener(EventType.RunPlayerComboSequenceEvent, param => PlayComboAnimation((ComboAnimContainer) param));
        this.AddListener(EventType.NotifyResumeAllComboSequenceEvent, param => _canPlay = true);
        this.AddListener(EventType.NotifyStopAllComboSequenceEvent, param => StartCoroutine(StopAllComboSequence((bool) param)));
    }

    /// <summary>
    /// Stop DOTween sequence for moving forward when attacking
    /// </summary>
    /// <param name="isPending"> isPending is false by default since "_canPlay" always set-ed by animation event,
    /// but on weapon switch (not an animation, YET). It should be done manually</param>
    /// <returns></returns>
    private IEnumerator StopAllComboSequence(bool isPending = false)
    {
        DOTween.Kill(_tweenObj);
        _canPlay = false;
        
        if (!isPending) yield break;
        
        yield return null;
        _canPlay = true;
    }
    
    private void PlayComboAnimation(ComboAnimContainer param)
    {
        if(!_canPlay) return;
        
        if (param.transform == null) {
            NCLogger.Log($"param.transform: {param.transform}", LogLevel.ERROR);
            return;
        }

        if (param.direction == Vector3.zero) {
            NCLogger.Log($"param.direction: {param.direction}", LogLevel.ERROR);
            return;
        }

        _tweenObj = param.transform;
        //change this into a coroutine so you can cancel it
        _tween = param.transform.DOMove(param.transform.position + param.direction * param.distance , param.time).SetEase(param.easeType);
    }
}