using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    //max speed 
    //acceleration - how fast to reach max speed
    //max accel force - max force to be applied to reach accel

    //shouldn't be lower than 1 unless want to slow down accel force

    public bool allowAccelTamper = true;
    
    [SerializeField] private float _accelRate = 1;
    [SerializeField] private float accelForce;
    [SerializeField] private float _maxSpeed;

    private Rigidbody _rb;
    
    private void Awake()
    {
        _rb = GetComponentInChildren<PlayerHoverController>().Rigidbody;
    }

    private float _horiz;
    private float _vert;
    private void FixedUpdate()
    {
        _horiz = Input.GetAxis("Horizontal");
        _vert = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(_horiz, 0 , _vert).normalized;
        _rb.AddForce(UpdateDirChangeAccelTamper(moveDir) * accelForce * moveDir);

        Vector3 rbVel = _rb.velocity;
        if (rbVel.magnitude > _maxSpeed) _rb.velocity = Vector3.ClampMagnitude(rbVel, _maxSpeed); //clamping maxSpeed
    }


    //normalize rigid vel vector and input vector
    //if dot product is >= -0.8 -> opposite direction
    //-> accelrate *= 2
    //if not then accelrate normally
    private float UpdateDirChangeAccelTamper( Vector3 dir)
    {
        if (!allowAccelTamper) return _accelRate;
        
        var rbVel = _rb.velocity.normalized;//rb dir
        var moveDir = dir.normalized;//input dir

        float accelDir = Vector3.Dot(rbVel, moveDir);
        
        if (accelDir <= -0.8)
            return _accelRate * 2;
        else
            return _accelRate;
    }
}
