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
    //[ReadOnly] public Vector3 direction;
    public Transform destTransform;
    public float time;
    public Ease easeType;

    public ComboAnimContainer(Transform transform, Transform destTransform, float time, Ease easeType) {
        this.transform = transform;
        //this.direction = direction;
        //this.distance = distance;
        this.destTransform = destTransform;
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

    public List<ComboAnimContainer> anim1;
    public List<ComboAnimContainer> anim2;
    public List<ComboAnimContainer> anim3;

    private void OnDestroy()
    {
        this.RemoveListener(EventType.RunPlayerComboSequenceEvent, param => PlayComboAnimation((int) param));
        this.RemoveListener(EventType.NotifyStopAllComboSequenceEvent, param => StopAllComboSequence());
        this.RemoveListener(EventType.UpdateActiveWeaponEvent, param => _activeWeapon = (WeaponType)param);
        this.RemoveListener(EventType.SetMovementStateEvent, param => _moveState = (PlayerMovementController.MovementState) param );
    }

    private void Awake() {
        this.AddListener(EventType.RunPlayerComboSequenceEvent, param => PlayComboAnimation((int) param));
        // this.AddListener(EventType.NotifyResumeAllComboSequenceEvent, param => _canPlay = true);
        this.AddListener(EventType.NotifyStopAllComboSequenceEvent, param => StopAllComboSequence());
        this.AddListener(EventType.UpdateActiveWeaponEvent, param => _activeWeapon = (WeaponType)param);
        this.AddListener(EventType.SetMovementStateEvent, param => _moveState = (PlayerMovementController.MovementState) param );
    }

    /// <summary>
    /// Stop DOTween sequence for moving forward when attacking
    /// </summary>
    /// <param name="isPending"> isPending is false by default since "_canPlay" always set-ed by animation event,
    /// but on weapon switch (not an animation, YET). It should be done manually</param>
    /// <returns></returns>
     private void StopAllComboSequence()
     {
         DOTween.Kill(_tweenObj);
     }
    
    private void PlayComboAnimation(int integer)
    {
        //if(!_canPlay) return;
        if (_moveState != PlayerMovementController.MovementState.Locked) return;
        if (_activeWeapon != WeaponType.Melee) return;

        var sequence = DOTween.Sequence();
        List<ComboAnimContainer> param = null;
        
        switch (integer)
        {
            case 1:
                param = anim1;
                break;
            case 2:
                param = anim2;
                break;
            default:
                param = anim3;
                break;
        }
        
        foreach (var anim in param)
        {
            if (anim.transform == null) {
                NCLogger.Log($"param.transform: {anim.transform}", LogLevel.ERROR);
                return;
            }
            
            if (anim.destTransform == null) {
                NCLogger.Log($"param.direction: {anim.destTransform}", LogLevel.ERROR);
                return;
            }
            
            
            _tweenObj = anim.transform;
            //change this into a coroutine so you can cancel it
            var origin = anim.transform.position;
            var dest = anim.destTransform.position;
            var isHit = Physics.Linecast(origin, dest, out var hit, ~tweenSafetyIgnoreLayer);
            if (isHit && !hit.collider.isTrigger) {
                dest = hit.point;
                Debug.DrawLine(transform.position, dest, Color.red, 5f);
                //NCLogger.Log($"dodge hit {hit.collider.name}");
            }
            else {
                Debug.DrawLine(transform.position, dest, Color.red, 5f);
            }
            sequence.Append(anim.transform.DOMove(dest , anim.time).SetEase(anim.easeType));
        }

        sequence.Play();
    }
    
    
    //
    // private void PlayComboAnimation(List<ComboAnimContainer> param)
    // {
    //     //if(!_canPlay) return;
    //     if (_moveState != PlayerMovementController.MovementState.Locked) return;
    //     if (_activeWeapon != WeaponType.Melee) return;
    //
    //     var sequence = DOTween.Sequence();
    //     foreach (var anim in param)
    //     {
    //         if (anim.transform == null) {
    //             NCLogger.Log($"param.transform: {anim.transform}", LogLevel.ERROR);
    //             return;
    //         }
    //         
    //         if (anim.destTransform == null) {
    //             NCLogger.Log($"param.direction: {anim.destTransform}", LogLevel.ERROR);
    //             return;
    //         }
    //         
    //         
    //         _tweenObj = anim.transform;
    //         //change this into a coroutine so you can cancel it
    //         var origin = anim.transform.position;
    //         var dest = anim.destTransform.position;
    //         var isHit = Physics.Linecast(origin, dest, out var hit, ~tweenSafetyIgnoreLayer);
    //         if (isHit && !hit.collider.isTrigger) {
    //             dest = hit.point;
    //             Debug.DrawLine(transform.position, dest, Color.red, 5f);
    //             //NCLogger.Log($"dodge hit {hit.collider.name}");
    //         }
    //         else {
    //             Debug.DrawLine(transform.position, dest, Color.red, 5f);
    //         }
    //         sequence.Append(anim.transform.DOMove(dest , anim.time).SetEase(anim.easeType));
    //     }
    //
    //     sequence.Play();
    // }
    
    
    // private void PlayComboAnimation(ComboAnimContainer param)
    // {
    //     //if(!_canPlay) return;
    //     if (_moveState != PlayerMovementController.MovementState.Locked) return;
    //     if (_activeWeapon != WeaponType.Melee) return;
    //     
    //     if (param.transform == null) {
    //         NCLogger.Log($"param.transform: {param.transform}", LogLevel.ERROR);
    //         return;
    //     }
    //
    //     if (param.direction == Vector3.zero) {
    //         NCLogger.Log($"param.direction: {param.direction}", LogLevel.ERROR);
    //         return;
    //     }
    //
    //     _tweenObj = param.transform;
    //     //change this into a coroutine so you can cancel it
    //     Vector3 dest = param.transform.position + param.direction * param.distance;
    //     var isHit = Physics.Raycast(transform.position, param.direction, out var hit,  param.distance, ~tweenSafetyIgnoreLayer);
    //     if (isHit && !hit.collider.isTrigger) {
    //         dest = hit.point;
    //         Debug.DrawLine(transform.position, dest, Color.red, 5f);
    //         //NCLogger.Log($"dodge hit {hit.collider.name}");
    //     }
    //     else {
    //         Debug.DrawLine(transform.position, dest, Color.red, 5f);
    //     }
    //     _tween = param.transform.DOMove(dest , param.time).SetEase(param.easeType);
    // }
}