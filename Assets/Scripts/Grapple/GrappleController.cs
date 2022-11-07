using System;
using System.Collections;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;


namespace Grapple
{
    public class GrappleController : MonoBehaviour
    {
        [Space]
        [SerializeField] private KeyCode grappleKey;

        [Header("Grapple Attributes")] 
        [SerializeField] private float range;
        [SerializeField] private float radius;
        [SerializeField] private List<LayerMask> masks;

        private LayerMask _grappleLayer;
        private Vector3 _castOrigin;
        private Camera _mainCam;
        private Vector3 _collisionPos;
        private RaycastHit[] _results = new RaycastHit[10];
        
        
        private void Start()
        {
            _mainCam = Camera.main;
            if (!_mainCam) NCLogger.Log("Can't find Camera.Main", LogLevel.ERROR);

            foreach (var mask in masks) {
                _grappleLayer += mask;
            }
        }
         
        
        
        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(grappleKey))
            {
                CastToGetGrappleLocation();
            }
        }

        private void CastToGetGrappleLocation()
        {
            var lookDir = _mainCam.transform.forward;
            _castOrigin = UpdateCastOrigin();
            
            var iteration = Physics.CapsuleCastNonAlloc(_castOrigin, _castOrigin, radius, lookDir, _results, range);

            if (iteration <= 0) {
                NCLogger.Log($"iteration of hits = {iteration}", LogLevel.INFO); 
                return;
            }
            
            RaycastHit hit = _results[0];
            for (var i = 0; i < iteration; i++) {
                if (_results[i].distance < hit.distance) hit = _results[i];
            }
            _collisionPos = hit.point;
            
        }


        private Vector3 UpdateCastOrigin()
        {
            return _mainCam.transform.position;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Camera.main.transform.position, radius);
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range);
            Gizmos.DrawWireSphere(_collisionPos, radius);
            //Gizmos.DrawRay(_castOrigin, );
        }
    }

}
