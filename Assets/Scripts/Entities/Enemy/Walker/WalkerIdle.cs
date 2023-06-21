using System;
using System.Collections;
using Audio;
using DG.Tweening;
using Entities.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WalkerIdle : EnemyState {
    [TitleGroup("Stats")]
    public float minimumDistanceToEdge;
    public float rotateDuration;

    [TitleGroup("Delay Before Wandering Again")]
    public float minDelayBeforeWalking;
    public float maxDelayBeforeWalking;

    [TitleGroup("Amount of Steps During Walk")]
    public float minWalkSteps;
    public float maxWalkSteps;
    
    [TitleGroup("Start Delay (?)")]
    public bool isStartDelayed;
    public float startDelay;

    [Title("State/Data")]
    [SerializeField] private WalkerHostile _nextState;
    [SerializeField] private AnimSerializedData _animData;
    
    [Title("Refs")] 
    [SerializeField] private EnemyMoveRootMotion _moveWithRootMotion;

    private AnimParam _walkingAnim;
    private bool _canWalk = true;
    private bool _canRotate = true;
    private bool _stateSwitched = false;

    public override EnemyState RunCurrentState() {
        if (_canWalk) {
            StartCoroutine(Delay());
        }

        if (!_canWalk) {
            if (NavMesh.FindClosestEdge(transform.position, out var hit, NavMesh.AllAreas)) {
                var dot = Vector3.Dot(Parent.forward, (hit.position - Parent.position).normalized);
                var dist = hit.distance;

                if (dot > 0.6f && dist < minimumDistanceToEdge && _canRotate) {
                    StartCoroutine(RotateEnemy());
                }
            }
        }

        if (detected && !_stateSwitched) {
            _stateSwitched = false;
            StopAllCoroutines();
            _nextState.ResetState();
            return _nextState;
        }
        return this;
    }
    
    
    private IEnumerator Delay() {
        _canWalk = false;
        if (_walkingAnim.name == null) _walkingAnim = _animData.hostileAnim.Find(anim => anim.name == "isWalking");
        yield return DelayedStart();
        Parent.DORotate(
            (Parent.rotation * Quaternion.Euler
                (0, Random.Range(Parent.rotation.y - 20, Parent.rotation.y + 20), 0).eulerAngles), rotateDuration);
        yield return new WaitForSeconds(rotateDuration);
        AudioManager.Instance.PlayClip(Parent.position, enemyBase.GetAudioClip(EnemyAudioType.Move));
        TriggerAnim(_walkingAnim);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length * Random.Range(minWalkSteps, maxWalkSteps));
        TriggerAnim(_walkingAnim);
        yield return new WaitForSeconds(Random.Range(minDelayBeforeWalking, maxDelayBeforeWalking));
        _canWalk = true;
    }

    private IEnumerator DelayedStart() {
        if (isStartDelayed) {
            isStartDelayed = false;
            yield return new WaitForSeconds(startDelay);
        }

        else {
            yield return null;
        }
    }

    private IEnumerator RotateEnemy() {
        _canRotate = false;
        //offset = 180 - 40 : 180 + 40 
        //180 is opposite axis 
        Parent.DORotate((Quaternion.AngleAxis(Random.Range(140, 220), Vector3.up) * Parent.rotation).eulerAngles, rotateDuration);
        yield return new WaitForSeconds(rotateDuration);
        _canRotate = true;
    }
}
