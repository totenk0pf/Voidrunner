using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    public Rigidbody Rigidbody
    {
        get => _rb != null ? _rb : GetComponentInChildren<Rigidbody>();
        private set => _rb ??= value; //set only if null
    }

    [SerializeField] [ReadOnly] private bool _isRayHit;
    [SerializeField] private float _rayLength; //length of raycast
    [SerializeField] private float _rideHeight; //optimal distance between player root and ground
    [SerializeField] private float _springStrength; //spring force 
    [SerializeField] private float _springDamp; //damp value to simulate spring strength losing power


    private void Awake()
    {
        _rb = Rigidbody;
    }

    private void Start()
    {
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ ;
    }

    private void Update()
    {
        UpdateCharacterHover();
    }

    private void UpdateCharacterHover()
    {
        _isRayHit = Physics.Raycast(transform.position, Vector3.down, out var hitInfo, _rayLength);

        if (_isRayHit)
        {
            Vector3 vel = _rb.velocity;
            Vector3 rayDir = transform.TransformDirection(Vector3.down);
            Rigidbody hitBody = hitInfo.rigidbody;
            Vector3 otherVel = hitBody ? hitBody.velocity : Vector3.zero; //velocity of object under player (OUP)

            //Dot product gives the magnitude and directional differences between 2 vectors.
            //if < 0 means 2 vectors have obtuse angle (opposite dir). if > 0 means they are sharp angle (same dir).
            //The more the magnitude rep the more magnitude of both vectors combined.
            
            float rayDirVel = Vector3.Dot(rayDir, vel); //dot between ray vector and player vector
            float otherDirVel = Vector3.Dot(rayDir, otherVel); // dot between ray vector and OUP vector

            float relVel = rayDirVel - otherDirVel; // combine the 2 magnitudes to get direction and magnitude of spring damp

            float offset = hitInfo.distance - _rideHeight;// distance offset to optimal ride height
            float springForce = (offset * _springStrength) - (relVel * _springDamp);//applied force = spring force - damp
            
            _rb.AddForce(rayDir * springForce);

            //Add force to the object standing below
            // if (hitBody) {
            //     hitBody.AddForceAtPosition(rayDir * -springForce, hitInfo.point);
            // }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //drawing raycast down
        Gizmos.color = Color.red; Gizmos.DrawRay(transform.position, Vector3.down * _rayLength);
    }
}
