using System;
using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Combat;
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
    //private bool _canPlay = true;
    private TweenerCore<Vector3, Vector3, VectorOptions> _tween;
    private Transform _tweenObj;
    [SerializeField] private LayerMask tweenSafetyIgnoreLayer;
    private WeaponType _activeWeapon;
    private PlayerMovementController.MovementState _moveState;

    private void Awake() {
        this.AddListener(EventType.RunPlayerComboSequenceEvent, param => PlayComboAnimation((ComboAnimContainer) param));
        // this.AddListener(EventType.NotifyResumeAllComboSequenceEvent, param => _canPlay = true);
        // this.AddListener(EventType.NotifyStopAllComboSequenceEvent, param => StartCoroutine(StopAllComboSequence((bool) param)));
        this.AddListener(EventType.UpdateActiveWeaponEvent, param => _activeWeapon = (WeaponType)param);
        this.AddListener(EventType.SetMovementStateEvent, param => _moveState = (PlayerMovementController.MovementState) param );
    }

    /// <summary>
    /// Stop DOTween sequence for moving forward when attacking
    /// </summary>
    /// <param name="isPending"> isPending is false by default since "_canPlay" always set-ed by animation event,
    /// but on weapon switch (not an animation, YET). It should be done manually</param>
    /// <returns></returns>
    // private IEnumerator StopAllComboSequence(bool isPending = false)
    // {
    //     DOTween.Kill(_tweenObj);
    //     _canPlay = false;
    //     
    //     if (!isPending) yield break;
    //     
    //     _canPlay = true;
    //}
    
    private void PlayComboAnimation(ComboAnimContainer param)
    {
        //if(!_canPlay) return;
        if (_moveState != PlayerMovementController.MovementState.Locked) return;
        if (_activeWeapon != WeaponType.Melee) return;
        
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
        Vector3 dest = param.transform.position + param.direction * param.distance;
        var isHit = Physics.Raycast(transform.position, param.direction, out var hit,  param.distance, ~tweenSafetyIgnoreLayer);
        if (isHit && !hit.collider.isTrigger) {
            dest = hit.point;
            Debug.DrawLine(transform.position, dest, Color.red, 5f);
            //NCLogger.Log($"dodge hit {hit.collider.name}");
        }
        else {
            Debug.DrawLine(transform.position, dest, Color.red, 5f);
        }
        _tween = param.transform.DOMove(dest , param.time).SetEase(param.easeType);
    }
}