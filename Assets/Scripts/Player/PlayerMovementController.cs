using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlayerMovementController : MonoBehaviour
{
    public enum MovementState
    {
        Normal,
        Roll,
        Slide
    }
    
    //max speed 
    //acceleration - how fast to reach max speed
    //max accel force - max force to be applied to reach accel

    //shouldn't be lower than 1 unless want to slow down accel force
    private Rigidbody _rb;
    public Rigidbody Rigidbody
    {
        get => _rb != null ? _rb : GetComponentInChildren<Rigidbody>();
        private set => _rb ??= value; //set only if null
    }

    public MovementState moveState;

    [SerializeField] private Transform playerVisualProto;
    
    [Space]
    [Header("Movement General")]
    public bool allowAccelTamper = true;
    [SerializeField] private float accelRate = 1;
    [SerializeField] private float accelForce;
    [SerializeField] private float maxSpeed;

    [Header("Movement Drag")] 
    public bool canDrag = true;
    public float rigidBodyDrag = 3;
    
    [Header("Slope Handling")] 
    [SerializeField] private float slopeCastDist;
    public float maxSlopeAngle;
    private RaycastHit _slopeHit;
    
    private Vector3 _moveDir;

    [Header("Roll Attributes")] 
    public bool toggleProtoRoll = true;
    public float rollAngleProto = 10;
    [Space]
    [SerializeField]  private KeyCode rollKey;
    [SerializeField] private float rollDistance;
    [SerializeField] private float rollTime;

    [Header("Slide Attributes")] 
    public bool toggleProtoSlide = true;
    [Space]
    [SerializeField] private KeyCode slideKey;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideDistance;
    

    private float _horiz;
    private float _vert;
    private void FixedUpdate()
    {
        UpdateMoveDir();
        if(moveState == MovementState.Normal) UpdateStrafe();
        if (canDrag) GivingDragOnSlope();
    }

    private void Update()
    {
        
        if(Input.GetKeyDown(rollKey)) ActionRoll(_moveDir);
        if(Input.GetKeyDown(slideKey)) ActionSlide(_moveDir);
    }


    private void UpdateStrafe()
    {
        if (OnSlope()) {
            _rb.AddForce(UpdateDirChangeAccelTamper(_moveDir) * accelForce * _moveDir);
        }
        else {
            _rb.AddForce(UpdateDirChangeAccelTamper(_moveDir) * accelForce * _moveDir);
        }
        
        ClampMaxSpeed();
    }

    private void ClampMaxSpeed()
    {
        if (OnSlope())
        {
            if (_rb.velocity.magnitude > maxSpeed)
                _rb.velocity = _rb.velocity.normalized * maxSpeed;
        }
        else
        {
            Vector3 rbVel = _rb.velocity;
            var relVel = new Vector3(rbVel.x, 0, rbVel.z);

            if (relVel.magnitude > maxSpeed)
            {
                relVel = Vector3.ClampMagnitude(rbVel, maxSpeed);
                _rb.velocity = new Vector3(relVel.x, _rb.velocity.y, relVel.z);
            }
        }
    }

    //normalize rigid vel vector and input vector
    //if dot product is >= -0.8 -> opposite direction
    //-> accelrate *= 2
    //if not then accelrate normally
    private float UpdateDirChangeAccelTamper( Vector3 dir)
    {
        if (!allowAccelTamper) return accelRate;
        
        var rbVel = _rb.velocity.normalized;//rb dir
        var moveDir = dir.normalized;//input dir

        float accelDir = Vector3.Dot(rbVel, moveDir);
        
        if (accelDir <= -0.8)
            return accelRate * 2;
        else
            return accelRate;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, slopeCastDist))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private void GivingDragOnSlope()
    {
        if (!OnSlope()) {
            _rb.drag = 0;
            _rb.useGravity = true;
            return;
        }
        
        float value = Vector3.Dot(_rb.velocity.normalized, Vector3.up);
        //going down
        if (value < 0) { 
            _rb.drag = rigidBodyDrag;
            _rb.useGravity = false;
        }
        else {
            _rb.drag = 0;
            _rb.useGravity = true;
        }

        if (_rb.velocity.magnitude < 0.3f)
            _rb.useGravity = true;

    }

    private Vector3 UpdateMoveDir()
    {
        _horiz = Input.GetAxis("Horizontal");
        _vert = Input.GetAxis("Vertical");
        _moveDir = transform.forward * _vert + transform.right * _horiz;
        
        if(OnSlope())
        {
            _moveDir = Vector3.ProjectOnPlane(_moveDir.normalized, _slopeHit.normal.normalized);
        }

        return _moveDir;
    }
    
    /// <summary>
    /// Roll the character
    /// - reset velocity, lerp to (distance + direction) point
    /// - roll visual towards said point
    /// </summary>
    /// <param name="moveDir"> normalized input direction vector </param>
    private void ActionRoll(Vector3 moveDir)
    {
        //check for movement and prevent roll stack
        if (moveDir.magnitude == 0 || moveState != MovementState.Normal) { return; }

        moveState = MovementState.Roll;
        
        _rb.velocity = Vector3.zero;
        Vector3 dest = transform.position + (moveDir) * rollDistance;
        Vector3 rollAxis = Vector3.Cross(moveDir, playerVisualProto.up);
        
        StartCoroutine(LerpRollRoutine(dest, rollAxis));
    }
    
    private IEnumerator LerpRollRoutine(Vector3 dest, Vector3 rollAxis)
    {
        var startPos = transform.position;
        float time = 0;
        
        while (time < rollTime)
        {
            transform.position = Vector3.Lerp(startPos, dest, time / rollTime);
            time += Time.deltaTime;
            
            if(toggleProtoRoll && moveState == MovementState.Roll)
                playerVisualProto.RotateAround(transform.position, rollAxis, rollAngleProto);
            
            
            yield return null;
        }
        
        transform.position = dest;
        playerVisualProto.up = Vector3.up;
        moveState = MovementState.Normal;
    }
    
    
    private void ActionSlide(Vector3 moveDir)
    {
        if (Mathf.Abs(_rb.velocity.magnitude - maxSpeed) > 0.5f || moveState != MovementState.Normal) { return; }

        moveState = MovementState.Slide;
        //var velMagCache = _rb.velocity.magnitude;
        Vector3 dest = transform.position + moveDir * slideDistance;
        Vector3 pointAxis = moveDir;

        StartCoroutine(LerpSlideRoutine(dest, pointAxis));

    }

    
    private IEnumerator LerpSlideRoutine(Vector3 dest, Vector3 pointAxis)
    {
        var startPos = transform.position;
        float time = 0;

        if (toggleProtoSlide && moveState == MovementState.Slide)
        {
            playerVisualProto.up = -pointAxis;
        }
        
        while (time < slideTime)
        {
            transform.position = Vector3.Lerp(startPos, dest, time / slideTime);
            time += Time.deltaTime;
            
            yield return null;
        }
        
        transform.position = dest;
        playerVisualProto.up = Vector3.up;
        moveState = MovementState.Normal;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _moveDir * 3);
    }
}


