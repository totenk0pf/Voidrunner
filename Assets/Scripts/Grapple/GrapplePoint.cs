using System;
using System.Collections;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grapple
{
    public class GrapplePoint : MonoBehaviour
    {
        // [SerializeField] private Vector3 castOrigin;
        // [SerializeField] private float radius;
        // [SerializeField] private float castDistance;
        // [SerializeField] private LayerMask ignoreMask;
        //
        // private Vector3 CastOrigin => castOrigin + transform.position;
        // private Vector3 _newPos;
        //
        //
        // public Tuple<bool, Vector3> GetAvailableSurface()
        // {
        //     NCLogger.Log($"get available");
        //     var returnTuple = new Tuple<bool, Vector3>(false, Vector3.zero);
        //     if(!Physics.SphereCast(CastOrigin, radius, -Vector3.up, out var hit, castDistance, ~ignoreMask)) return returnTuple;
        //     NCLogger.Log($"hit smt");
        //     if (Vector3.Angle(hit.normal, Vector3.up) < 10) returnTuple = new Tuple<bool, Vector3>(true, hit.point);
        //     _newPos = hit.point;
        //     return returnTuple;
        // }
        //
        // private void OnDrawGizmosSelected()
        // {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawWireSphere(CastOrigin, radius);
        //     Gizmos.DrawWireSphere(CastOrigin + Vector3.down * castDistance, radius);
        //     Gizmos.DrawLine(CastOrigin, CastOrigin + Vector3.down * castDistance);
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawSphere(_newPos, .3f);
        // }
    }
}