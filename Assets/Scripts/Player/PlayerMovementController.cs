using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Logging;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using EventType = Core.Events.EventType;

public class PlayerMovementController : MonoBehaviour
{
    public enum MovementState
    {
        Normal,
        Dodge,
        Grappling
    }
    
    //max speed 
    //acceleration - how fast to reach max speed
    //max accel force - max force to be applied to reach accel

    //shouldn't be lower than 1 unless want to slow down accel force
    private Rigidbody _rb;
    private Rigidbody Rb
    {
        get {
            if (!_rb) _rb = GetComponent<Rigidbody>();
            return _rb;
        }
    }

    [SerializeField] private MovementState moveState;

    [SerializeField] private Transform playerVisualProto;
    
    [Space]
    [Header("Movement General")]
    public bool allowAccelTamper = true;
    [SerializeField] private float accelRate = 1;
    [SerializeField] private float accelForce;
    [SerializeField] private float maxSpeed;

    private bool _canGravity = true;
    [Header("Custom Gravity and Ground Check")] 
    [SerializeField] private float gravityAcceleration;
    [SerializeField] private float gravityScale;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private float groundCastDist;
    
    [Header("Movement Drag")] 
    [SerializeField] private bool canDrag = true;
    [SerializeField] private float slopeDrag = 3f;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;

    [Header("Slope Handling")] 
    [SerializeField] private float slopeCastDist;
    public float maxSlopeAngle;
    private RaycastHit _slopeHit;
    
    private Vector3 _moveDir;
    
    [Header("Dodge Attributes")] 
    public bool toggleProtoDodge = true;
    [Space]
    [SerializeField] private KeyCode dodgeKey;
    [SerializeField] private float dodgeTime;
    [SerializeField] private float dodgeDistance;

    
    private Dictionary<KeyCode, float> _inputToDirDict;
    private Vector3 _dodgeDir;
    private float _horiz;
    private float _vert;
    private bool _isGrounded;

    private void Awake()
    {
        EventDispatcher.Instance.AddListener(EventType.GetMovementStateEvent, param => GetMovementState());
        EventDispatcher.Instance.AddListener(EventType.SetMovementStateEvent, param => UpdateMovementState((MovementState) param));
        EventDispatcher.Instance.AddListener(EventType.RequestIsOnGroundEvent, param => EventDispatcher.Instance.FireEvent(EventType.ReceiveIsOnGroundEvent, _isGrounded));
        
        _inputToDirDict = new Dictionary<KeyCode, float>() {
            {KeyCode.W, 0},
            {KeyCode.S, 180},
            {KeyCode.A, -90},
            {KeyCode.D, 90}
        };
        _canGravity = true;
        Rb.useGravity = false;
    }
    
    private void FixedUpdate()
    {
        UpdateMoveDir();
        if(moveState == MovementState.Normal) UpdateStrafe();
        if (canDrag) ApplyDrag();
        if(_canGravity) CustomGravity();
    }

    private void Update()
    {
        foreach (var input in _inputToDirDict.Keys.Where(Input.GetKey)) {
            _dodgeDir = Quaternion.AngleAxis(_inputToDirDict[input], Vector3.up) * transform.forward;
            break;
        }
        if(Input.GetKeyDown(dodgeKey)) ActionDodge(_dodgeDir);
    }

    #region Movement Base
    private void UpdateStrafe()
    {
        if (OnSlope()) {
            Rb.AddForce(UpdateDirChangeAccelTamper(_moveDir) * accelForce * _moveDir, ForceMode.Acceleration);
        }
        else {
            Rb.AddForce(UpdateDirChangeAccelTamper(_moveDir) * accelForce * _moveDir, ForceMode.Acceleration);
        }
        
        ClampMaxSpeed();
    }

    private void ClampMaxSpeed()
    {
        if (OnSlope())
        {
            if (Rb.velocity.magnitude > maxSpeed)
                Rb.velocity = Rb.velocity.normalized * maxSpeed;
        }
        else
        {
            Vector3 rbVel = Rb.velocity;
            var relVel = new Vector3(rbVel.x, 0, rbVel.z);

            if (relVel.magnitude > maxSpeed)
            {
                relVel = Vector3.ClampMagnitude(rbVel, maxSpeed);
                Rb.velocity = new Vector3(relVel.x, Rb.velocity.y, relVel.z);
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
        
        var rbVel = Rb.velocity.normalized;//rb dir
        var moveDir = dir.normalized;//input dir

        float accelDir = Vector3.Dot(rbVel, moveDir);
        
        if (accelDir <= -0.8)
            return accelRate * 2;
        else
            return accelRate;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, slopeCastDist, ~ignoreMask))
        {
            if (_slopeHit.collider.isTrigger) return false;
            
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private void ApplyDrag()
    {
        if (!OnSlope()) {
            Rb.drag = groundDrag;
            _canGravity = true;
            //Rb.useGravity = true;
            return;
        }
        
        float value = Vector3.Dot(Rb.velocity.normalized, Vector3.up);
        //going down
        if (value < 0) { 
            Rb.drag = slopeDrag;
            _canGravity = false;
            //Rb.useGravity = false;
        }
        else {
            Rb.drag = groundDrag;
            _canGravity = true;
            //Rb.useGravity = true;
        }

        if (Rb.velocity.magnitude < 0.3f)
            _canGravity = true;
            //Rb.useGravity = true;

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
    #endregion
    
    #region Movement Abilities (Outdated)
    
    /// <summary>
    /// Roll the character
    /// - reset velocity, lerp to (distance + direction) point
    /// - roll visual towards said point
    /// </summary>
    /// <param name="moveDir"> normalized input direction vector </param>
    // private void ActionRoll(Vector3 moveDir)
    // {
    //     //check for movement and prevent roll stack
    //     if (moveDir.magnitude == 0 || moveState != MovementState.Normal) { return; }
    //
    //     moveState = MovementState.Roll;
    //     
    //     Rb.velocity = Vector3.zero;
    //     Vector3 dest = transform.position + (moveDir) * rollDistance;
    //     Vector3 rollAxis = Vector3.Cross(moveDir, playerVisualProto.up);
    //     
    //     StartCoroutine(LerpRollRoutine(dest, rollAxis));
    // }
    //
    // private IEnumerator LerpRollRoutine(Vector3 dest, Vector3 rollAxis)
    // {
    //     var startPos = transform.position;
    //     float time = 0;
    //     
    //     while (time < rollTime)
    //     {
    //         transform.position = Vector3.Lerp(startPos, dest, time / rollTime);
    //         time += Time.deltaTime;
    //         
    //         if(toggleProtoRoll && moveState == MovementState.Roll)
    //             playerVisualProto.RotateAround(transform.position, rollAxis, rollAngleProto);
    //         
    //         
    //         yield return null;
    //     }
    //     
    //     transform.position = dest;
    //     playerVisualProto.up = Vector3.up;
    //     moveState = MovementState.Normal;
    // }
    //
    //
    // private void ActionSlide(Vector3 moveDir)
    // {
    //     if (Mathf.Abs(Rb.velocity.magnitude - maxSpeed) > 0.5f || moveState != MovementState.Normal) { return; }
    //
    //     moveState = MovementState.Slide;
    //     //var velMagCache = Rb.velocity.magnitude;
    //     Vector3 dest = transform.position + moveDir * slideDistance;
    //     Vector3 pointAxis = moveDir;
    //
    //     StartCoroutine(LerpSlideRoutine(dest, pointAxis));
    //
    // }
    //
    //
    // private IEnumerator LerpSlideRoutine(Vector3 dest, Vector3 pointAxis)
    // {
    //     var startPos = transform.position;
    //     float time = 0;
    //
    //     if (toggleProtoSlide && moveState == MovementState.Slide)
    //     {
    //         playerVisualProto.up = -pointAxis;
    //     }
    //     
    //     while (time < slideTime)
    //     {
    //         transform.position = Vector3.Lerp(startPos, dest, time / slideTime);
    //         time += Time.deltaTime;
    //         
    //         yield return null;
    //     }
    //     
    //     transform.position = dest;
    //     playerVisualProto.up = Vector3.up;
    //     moveState = MovementState.Normal;
    //
    // }
    #endregion
    
    #region Movement Abilities
    private void ActionDodge(Vector3 moveDir)
    {
        if (Rb.velocity.magnitude == 0 || moveState != MovementState.Normal) { return; }
    
        moveState = MovementState.Dodge;
        //var velMagCache = Rb.velocity.magnitude;
        Vector3 dest = transform.position + moveDir * dodgeDistance;
        Vector3 rollAxis = Vector3.Cross(moveDir, playerVisualProto.up);
    
        StartCoroutine(LerpDodgeRoutine(dest, rollAxis));
    }
    
    
    private IEnumerator LerpDodgeRoutine(Vector3 dest, Vector3 rollAxis)
    {
        NCLogger.Log($"Dir: {_dodgeDir}", LogLevel.INFO);
        var startPos = transform.position;
        var rbVel = Rb.velocity;
        float time = 0;
    
        if (toggleProtoDodge && moveState == MovementState.Dodge) {
            playerVisualProto.Rotate(rollAxis, -30f, Space.Self);
        }
        
        while (time < dodgeTime) {
            transform.position = Vector3.Lerp(startPos, dest, time / dodgeTime);
            time += Time.deltaTime;
            
            yield return null;
        }
        
        transform.position = dest;
        playerVisualProto.up = Vector3.up;
        moveState = MovementState.Normal;
        Rb.velocity = rbVel;
    }
    #endregion
    
    private bool IsOnGround()
    {
        bool val;
        if (!Physics.Raycast(transform.position, -Vector3.up, out var hit, groundCastDist, ~ignoreMask))
        {
            val = false;
            _isGrounded = val;
            return val;
        }
        val = !hit.collider.isTrigger;

        
        _isGrounded = val;
        return val;
    }

    private void CustomGravity()
    {
        var gravityVector = IsOnGround() ? 
            (-9.8f * 1 * Vector3.up) : 
            (-gravityAcceleration * gravityScale * Vector3.up);
        
        Rb.AddForce(gravityVector, ForceMode.Acceleration);
    }

    private void GetMovementState() {
        EventDispatcher.Instance.FireEvent(EventType.SetMovementStateEvent, moveState);
    }

    private void UpdateMovementState(MovementState state)
    {
        moveState = state;
        if (state == MovementState.Grappling) {
            _rb.velocity = Vector3.zero;
        }
    }

    public IEnumerator GravityDampRoutine(float dampDuration) {
        if (dampDuration > 0) {
            var originalScale = gravityScale;
            var currentTime = 0f;
            gravityScale = 0;
            while (currentTime < dampDuration || gravityScale < originalScale) {
                gravityScale = (currentTime/dampDuration) * originalScale;
                currentTime += Time.deltaTime;
                yield return null;
            }
            gravityScale = originalScale;
            yield return null;  
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _moveDir * 3);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, _dodgeDir * 5);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, groundCastDist * Vector3.down);
    }
}


