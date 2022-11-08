using System;
using System.Collections;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
using UnityEngine.Serialization;


namespace Grapple
{
    public class GrappleController : MonoBehaviour
    {
        [Space]
        [SerializeField] private KeyCode grappleKey;

        [Header("Grapple Attributes")] 
        [SerializeField] private float range;
        [SerializeField] private float radius;
        [SerializeField] private float maxAngleAA = 5f;

        [SerializeField] private LayerMask grappleLayer;
        [SerializeField] private LayerMask ignoreLayer;
        private Vector3 _castOrigin;
        private Camera _mainCam;
        private Vector3 _collisionPos;

        public GameObject currentGrappleCollider;
        
        private void Start()
        {
            _mainCam = Camera.main;
            if (!_mainCam) NCLogger.Log("Can't find Camera.Main", LogLevel.ERROR);
            
        }


        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(grappleKey))
            {
                CastToGetGrappleLocation();
            }
        }

        private bool CastToGetGrappleLocation()
        {
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
            
            //if grapple point is closest -> success
            if (checkIndex == 0) return tupleSuccess;

            //secondary raycast to make sure the grapple point is in field of vision
            if (!Physics.Linecast(_castOrigin, hit1.collider.transform.position, out hit2, ~ignoreLayer)) return tupleFail;
            if(hit2.collider) if(!IsInGrappleMask(hit2.collider.gameObject.layer)) return tupleFail;

            //angle between cam.fwd and direction(cam to hit1.point)
            var angle = Vector3.Angle(hit1.point - _castOrigin, _mainCam.transform.forward);
            //return based on angle
            if (!(angle <= maxAngleAA)) return tupleFail;
            currentGrappleCollider = hit2.collider.gameObject; 
            return hit2.collider == hit1.collider ? tupleSuccess : tupleFail;
        }

        private bool IsInGrappleMask(int layer) {
            return grappleLayer == (grappleLayer | (1 << layer));
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
        }
    }

}
    