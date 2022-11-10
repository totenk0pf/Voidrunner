using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Serialization;
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
        [Header("Grapple Enemy Attributes")]
        [SerializeField] private LayerMask enemyLayer;
        [Space]
        [Header("Grapple Point Attributes")]
        [SerializeField] private LayerMask grapplePointLayer;
        
        private Dictionary<LayerMask, GrappleType> _grappleCondition;
        private Vector3 GrappleHaltPosition => transform.position + transform.forward * grappleHaltOffsetZ;
        
        
        private Vector3 _castOrigin;
        private Camera _mainCam;
        private Vector3 _collisionPos;
        private LineRenderer _lineRenderer;
        private RaycastHit _currentGrappleHit;
        private PlayerMovementController.MovementState _moveState;
        [Space]
        public GameObject currGrappleObj;

        private void Awake()
        {
            EventDispatcher.Instance.AddListener(EventType.SetMovementStateEvent, param => UpdateMoveState((PlayerMovementController.MovementState) param));
            
            if (!GetComponent<LineRenderer>()) transform.AddComponent<LineRenderer>();
            _lineRenderer = GetComponent<LineRenderer>();
            
            _grappleCondition = new Dictionary<LayerMask, GrappleType>() {
                {enemyLayer, GrappleType.EnemyToPlayer},
                {grappleLayer, GrappleType.PlayerToPoint}
            };
        }

        private void Start()
        {
            _mainCam = Camera.main;
            if (!_mainCam) NCLogger.Log("Can't find Camera.Main", LogLevel.ERROR);
        }


        // Update is called once per frame
        private void Update()
        {
            if(_currentGrappleHit.collider) currGrappleObj = _currentGrappleHit.collider.gameObject;
            if (Input.GetKeyDown(grappleKey))
            {
                if(!CastToGetGrappleLocation()) return;
                Grapple();
            }
        }

        private bool CastToGetGrappleLocation()
        {
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
                if (IsObjectBehindPlayer(results[i].collider.transform))
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
        private Tuple<bool, Vector3> GetHitPointAimAssisted(RaycastHit hit1, int checkIndex, Vector3 lookDir)
        {
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

        private bool Grapple()
        {
            if (!_currentGrappleHit.collider) return false;
            NCLogger.Log($"'pass collider filter");
            var type = GetGrappleType(_currentGrappleHit.collider.gameObject.layer);
            
            
            if (type == GrappleType.None) return false;
            NCLogger.Log($"'pass layer filter");
            
            switch (type)
            {
                case GrappleType.EnemyToPlayer:
                    break;
                case GrappleType.PlayerToPoint:
                    EventDispatcher.Instance.FireEvent(EventType.SetMovementStateEvent, PlayerMovementController.MovementState.Grappling);
                    StartCoroutine(PlayerToPointRoutine());
                    return true;
                default:
                    return false;
            }

            return false;
        }


        private IEnumerator PlayerToPointRoutine() {
            var startPos = transform.position;
            var dist = Vector3.Distance(_currentGrappleHit.point, GrappleHaltPosition);
            var dir = (_currentGrappleHit.point - GrappleHaltPosition).normalized;
            var endPos = startPos + (dist * dir);
            
            for (float i = 0.0f; i < 1.0f; i += (grappleSpeed * Time.deltaTime) / dist) {
                transform.position = Vector3.Lerp(startPos, endPos, i);
                yield return null;
            }
            
            EventDispatcher.Instance.FireEvent(EventType.SetMovementStateEvent, PlayerMovementController.MovementState.Normal);
            _currentGrappleHit = new RaycastHit();
            yield return null;
        }



        private void UpdateMoveState(PlayerMovementController.MovementState currState) {
            _moveState = currState;
        }
        
        private bool IsInGrappleMask(int layer) {
            return grappleLayer == (grappleLayer | (1 << layer));
        }

        private bool IsObjectBehindPlayer(Transform objectT)
        {
            var forward = transform.TransformDirection(Vector3.forward);
            var toOther = objectT.position - transform.position;

            return Vector3.Dot(forward, toOther) < 0;
        }
        
        private GrappleType GetGrappleType(int layer)
        {
            foreach(var (layerMask, type) in _grappleCondition) {
                if (layerMask == (layerMask | (1 << layer))) return type;
            }

            return GrappleType.None;
        }
        
        
        
        
        private Vector3 UpdateCastOrigin() {
            return _mainCam.transform.position;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Camera.main.transform.position, radius);
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range);
            Gizmos.DrawWireSphere(_collisionPos, radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GrappleHaltPosition, .3f);
        }
    }

}
    