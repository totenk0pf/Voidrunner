using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapple
{
    public class GrapplePoint : MonoBehaviour
    {
        [SerializeField] private Vector3 dimension;
        [SerializeField] private Vector3 castOrigin = Vector3.zero;
        [SerializeField] private float castDistance;
        [SerializeField] private LayerMask ignoreMask;
        
        private void Awake()
        {
            if (castOrigin == Vector3.zero) castOrigin = transform.position;
        }

        public Tuple<bool, Vector3> GetAvailableSurface()
        {
            var returnTuple = new Tuple<bool, Vector3>(false, Vector3.zero);
            Physics.BoxCast(transform.position, dimension, Vector3.down, Quaternion.identity, castDistance, ~ignoreMask);
            
            
            
            return returnTuple;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube();
        }
    }
}