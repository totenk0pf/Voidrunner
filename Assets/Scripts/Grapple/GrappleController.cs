using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using Unity.VisualScripting;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Grapple
{
    [Serializable]
    public enum GrappleType
    {
        None,
        PlayerToPoint,
        EnemyToPlayer
    }
    
    //[RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(PlayerMovementController))]
    public class GrappleController : MonoBehaviour
    {
        [Space]
        [SerializeField] private KeyCode grappleKey;

        [Header("Grapple Attributes")] 
        [SerializeField] private float range;
        [SerializeField] private float radius;
        [SerializeField] private float maxAngleAA = 5f;
        [Space]
        [SerializeField] private LayerMask grappleLayer;
        [SerializeField] private LayerMask ignoreLayer;
        [Space] 
        [SerializeField] private float grappleHaltOffsetZ;
        [SerializeField] private float grappleSpeed = 5f;
        [Space] 
        [SerializeField] private float distIgnoreCheck = 1;
        [Header("Grapple Enemy Attributes")]
        [SerializeField] private LayerMask enemyLayer;
        [Space]
        [Header("Grapple Point Attributes")]
        [SerializeField] private LayerMask grapplePointLayer;

        [Header("Post-Grapple Attributes")] 
        [SerializeField] private float gravityDampDuration;
        [SerializeField] private float momentumForce;
        [SerializeField] private ForceMode forceMode;
        [SerializeField] private float enemyMomentumForce;
        [SerializeField] private ForceMode enemyForceMode;
        private Dictionary<LayerMask, GrappleType> _grappleCondition;
        private Vector3 GrappleHaltPosition => transform.position + transform.forward * grappleHaltOffsetZ;
        private Rigidbody _rb;
        private Rigidbody Rigidbody {
            get {
                if (!_rb) _rb = GetComponent<Rigidbody>();
                return _rb;
            }
        }
        
        private Vector3 _castOrigin;
        private Camera _mainCam;
        private Vector3 _collisionPos;
        private LineRenderer _lr;
        private PlayerMovementController _controller;
        private RaycastHit _currentGrappleHit;
        private PlayerMovementController.MovementState _moveState;
        private bool _isOnGround;
        private IEnumerator _enemyToPlayerRoutine;
        private IEnumerator _playerToPointRoutine;
        private EnemyBase _currentGrappledEnemy;
        private Vector3 PlayerHeightOffset => new Vector3(0, 1, 0) * transform.localScale.y;
        [Space]
        public GameObject currGrappleObj;

        private void Awake()
        {
            EventDispatcher.Instance.AddListener(EventType.SetMovementStateEvent, param => UpdateMoveState((PlayerMovementController.MovementState) param));
            EventDispatcher.Instance.AddListener(EventType.ReceiveIsOnGroundEvent, param => UpdateIsOnGround((bool)param));
            
            if (!GetComponent<LineRenderer>()) transform.AddComponent<LineRenderer>();
            _lr = GetComponent<LineRenderer>();
            _controller = GetComponent<PlayerMovementController>();
            
            _grappleCondition = new Dictionary<LayerMask, GrappleType>() {
                {enemyLayer, GrappleType.EnemyToPlayer},
                {grappleLayer, GrappleType.PlayerToPoint}
            };
        }

        private void Start() {
            _mainCam = Camera.main;
            if (!_mainCam) NCLogger.Log("Can't find Camera.Main", LogLevel.ERROR);

            Vector3[] positions = new Vector3[2];
            _lr.positionCount = positions.Length;
            _lr.SetPositions(positions);

            _lr.enabled = false;
        }
        
        
        // Update is called once per frame
        private void Update() {
            if(_currentGrappleHit.collider) currGrappleObj = _currentGrappleHit.collider.gameObject;
            if (Input.GetKeyDown(grappleKey)) {
                if (_moveState != PlayerMovementController.MovementState.Grappling) {
                    if(!CastToGetGrappleLocation()) return;
                    Grapple();
                }
                else//is during grappling
                {
                    //cancel grappling TODO: When cancel grappling on any object, have them keep momentum to them.
                    CancelGrapple();
                }
            }
        }

        private bool CastToGetGrappleLocation() {
            EventDispatcher.Instance.FireEvent(EventType.GetMovementStateEvent);
            if (_moveState != PlayerMovementController.MovementState.Normal) return false;
            
            var lookDir = _mainCam.transform.forward;
            _castOrigin = UpdateCastOrigin();

            var results = Physics.CapsuleCastAll(_castOrigin, _castOrigin, radius, lookDir, range, ~ignoreLayer);
            if (results.Length <= 0) {
                _collisionPos = Vector3.zero;
                return false;
            }
            
            //Sort by distance, from closest to furthest
            Array.Sort(results, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

            var hit = new RaycastHit();
            var checkIndex = 0;
            
            for (var i = 0; i < results.Length; i++) {
                if (!IsInGrappleMask(results[i].collider.gameObject.layer)) 
                    continue;
                if (!IsObjectAvailable(results[i]))
                    continue;

                hit = results[i];
                checkIndex = i;
                break;
            }
            if (!hit.collider) return false;
            var tuple = GetHitPointAimAssisted(hit, checkIndex, lookDir);

            _collisionPos = tuple.Item1 ? tuple.Item2 : Vector3.zero;
            
            return tuple.Item1;
        }
        
        private Tuple<bool, Vector3> GetHitPointAimAssisted(RaycastHit hit1, int checkIndex, Vector3 lookDir) {
            var tupleSuccess = new Tuple<bool, Vector3>(true, hit1.point);
            var tupleFail = new Tuple<bool, Vector3>(false, Vector3.zero);
            var hit2 = new RaycastHit();
            _currentGrappleHit = hit1; 
            
            //if grapple point is closest -> success
            if (checkIndex == 0) return tupleSuccess;

            //secondary raycast to make sure the grapple point is in field of vision
            if (!Physics.Linecast(_castOrigin, hit1.collider.transform.position, out hit2, ~ignoreLayer)) return tupleFail;
            if(hit2.collider) if(!IsInGrappleMask(hit2.collider.gameObject.layer)) return tupleFail;

            //angle between cam.fwd and direction(cam to hit1.point)
            var angle = Vector3.Angle(hit1.point - _castOrigin, _mainCam.transform.forward);
            //return based on angle
            if (!(angle <= maxAngleAA)) return tupleFail;
            _currentGrappleHit = hit2; 
            return hit2.collider == hit1.collider ? tupleSuccess : tupleFail;
        }

        private bool Grapple() { 
            if (!_currentGrappleHit.collider) return false;
            var type = GetGrappleType(_currentGrappleHit.collider.gameObject.layer);
            if (type == GrappleType.None) return false;

            StopAllCoroutines();
            switch (type)
            {
                case GrappleType.EnemyToPlayer:
                    _currentGrappledEnemy = _currentGrappleHit.collider.GetComponent<EnemyBase>();
                    if (!_currentGrappledEnemy.CanPull) return false;
                    EventDispatcher.Instance.FireEvent(EventType.RequestIsOnGroundEvent);
                    if (!_isOnGround) return false;
                    EventDispatcher.Instance.FireEvent(EventType.SetMovementStateEvent, PlayerMovementController.MovementState.Grappling);
                    _enemyToPlayerRoutine = EnemyToPlayerRoutine();
                    StartCoroutine(_enemyToPlayerRoutine);
                    break;
                case GrappleType.PlayerToPoint:
                    EventDispatcher.Instance.FireEvent(EventType.SetMovementStateEvent, PlayerMovementController.MovementState.Grappling);
                    _playerToPointRoutine = PlayerToPointRoutine();
                    StartCoroutine(_playerToPointRoutine);
                    return true;
                default:
                    return false;
            }

            return false;
        }

        private void CancelGrapple()
        {
            if (_playerToPointRoutine != null) {
                StopCoroutine(_playerToPointRoutine);
                ResetGrapple_PlayerToPoint(isCanceled: true);
            }
            if (_enemyToPlayerRoutine != null) {
                StopCoroutine(_enemyToPlayerRoutine);
                ResetGrapple_EnemyToPlayer(isCanceled: true);
            }
            _lr.enabled = false;
        }

        private void ResetGrapple_EnemyToPlayer(bool isCanceled = false)
        {
            if (isCanceled) {
                var dir = (transform.position - _currentGrappledEnemy.transform.position).normalized;
                _currentGrappledEnemy.Rigidbody.AddForce(dir*enemyMomentumForce, enemyForceMode);
            }
            
            EventDispatcher.Instance.FireEvent(EventType.SetMovementStateEvent, PlayerMovementController.MovementState.Normal);
            _currentGrappleHit = new RaycastHit();
            _lr.enabled = false;

            if(!_currentGrappledEnemy) NCLogger.Log($"_currentGrappledEnemy Null Object Exception", LogLevel.ERROR);
            _currentGrappledEnemy.OnRelease();
            _enemyToPlayerRoutine = null;
        }

        private void ResetGrapple_PlayerToPoint(bool isCanceled = false)
        {
            if (isCanceled) {
                var dir = (_currentGrappleHit.point - transform.position).normalized;
                StartCoroutine(_controller.GravityDampRoutine(gravityDampDuration));
                Rigidbody.AddForce(dir*momentumForce, forceMode);
            }
            
            EventDispatcher.Instance.FireEvent(EventType.SetMovementStateEvent, PlayerMovementController.MovementState.Normal);
            _currentGrappleHit = new RaycastHit();
            _lr.enabled = false;
            _playerToPointRoutine = null;
        }

        #region Grapple Routines
        private IEnumerator EnemyToPlayerRoutine() {
            var startPos = _currentGrappleHit.point;
            var dist = Vector3.Distance( _currentGrappleHit.point, GrappleHaltPosition);
            var dir = (GrappleHaltPosition - _currentGrappleHit.point).normalized;
            var endPos = startPos + (dist * dir);

            if(!_currentGrappledEnemy) NCLogger.Log($"_currentGrappledEnemy Null Object Exception", LogLevel.ERROR);
            _currentGrappledEnemy.OnGrappled();
            
            _lr.enabled = true;
            _lr.SetPosition(0, GrappleHaltPosition);

            for (var i = 0.0f; i < 1.0f; i += (grappleSpeed * Time.deltaTime) / dist) {
                _lr.SetPosition(1, _currentGrappleHit.transform.position);
                _currentGrappleHit.collider.transform.position = Vector3.Lerp(startPos, endPos, i);
                yield return null;
            }

            ResetGrapple_EnemyToPlayer();
            yield return null;
        }

        private IEnumerator PlayerToPointRoutine() {
            var startPos = transform.position;
            var dist = Vector3.Distance( _currentGrappleHit.point, GrappleHaltPosition);
            var dir = ( _currentGrappleHit.point - GrappleHaltPosition).normalized;
            var endPos = startPos + (dist * dir);
            
            _lr.enabled = true;
            _lr.SetPosition(1, _currentGrappleHit.transform.position);
            
            for (var i = 0.0f; i < 1.0f; i += (grappleSpeed * Time.deltaTime) / dist) {
                _lr.SetPosition(0, GrappleHaltPosition);
                transform.position = Vector3.Lerp(startPos, endPos, i);
                yield return null;
            }

            ResetGrapple_PlayerToPoint();
            yield return null;
        }
        #endregion

        #region Helper Function
        private void UpdateMoveState(PlayerMovementController.MovementState currState) {
            _moveState = currState;
        }
        
        private bool IsInGrappleMask(int layer) {
            return grappleLayer == (grappleLayer | (1 << layer));
        }

        private bool IsObjectAvailable(RaycastHit hitTemp) {
            //Checking if grapple point is behind player
            var forward = transform.TransformDirection(Vector3.forward);
            var toOther = hitTemp.collider.transform.position - transform.position;
            if (Vector3.Dot(forward, toOther) < 0) return false;
            
            //Checking if grapple is at suitable angle to grab
            var angle = Vector3.Angle(hitTemp.point - _castOrigin, _mainCam.transform.forward);
            if (!(angle <= maxAngleAA)) return false;

            //Check dist to ignore
            var dist = Vector3.Distance(hitTemp.transform.position, transform.position);
            if (dist <= distIgnoreCheck) return false;

            return true;
        }
        
        private GrappleType GetGrappleType(int layer) {
            foreach(var (layerMask, type) in _grappleCondition) {
                if (layerMask == (layerMask | (1 << layer))) return type;
            }

            return GrappleType.None;
        }

        private Vector3 UpdateCastOrigin() {
            return _mainCam.transform.position;
        }

        private void UpdateIsOnGround(bool isOnGround)
        {
            _isOnGround = isOnGround;
        }
        #endregion
        
        #region Gizmos
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Camera.main.transform.position, radius);
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range);
            Gizmos.DrawWireSphere(_collisionPos, radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GrappleHaltPosition, .3f);
        }
        #endregion
    }

}
    