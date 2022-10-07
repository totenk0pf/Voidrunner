using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    public MovementState moveState;

    [SerializeField] private Transform playerVisualProto;
    
    [Space]
    [Header("Movement General")]
    public bool allowAccelTamper = true;
    [SerializeField] private float accelRate = 1;
    [SerializeField] private float accelForce;
    [SerializeField] private float maxSpeed;

    private Rigidbody _rb;
    private Vector3 _moveDir;

    [Header("Roll Attributes")] 
    public bool toggleProtoRoll = true;
    public float rollAngleProto = 10;
    [Space]
    [SerializeField]  private KeyCode rollKey;
    [SerializeField] private float rollDistance;
    [SerializeField] private float rollTime;
    [SerializeField] private float routineInterval;

    [Header("Slide Attributes")] 
    [SerializeField] private KeyCode slideKey;
    [SerializeField] private float slideTime;

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
        _moveDir = new Vector3(_horiz, 0 , _vert).normalized;
        
        
        
        if(moveState == MovementState.Normal) UpdateStrafe(_moveDir);
    }

    private void Update()
    {
        if(Input.GetKeyDown(rollKey)) ActionRoll(_moveDir);
    }


    private void UpdateStrafe(Vector3 moveDir)
    {
        _rb.AddForce(UpdateDirChangeAccelTamper(moveDir) * accelForce * moveDir);

        Vector3 rbVel = _rb.velocity;
        if (rbVel.magnitude > maxSpeed) _rb.velocity = Vector3.ClampMagnitude(rbVel, maxSpeed); //clamping maxSpeed
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
    
    
    /// <summary>
    /// Roll the character
    /// - reset velocity, lerp to (distance + direction) point
    /// - roll visual towards said point
    /// </summary>
    /// <param name="moveDir"> normalized input direction vector </param>
    private void ActionRoll(Vector3 moveDir)
    {
        //check for movement and prevent roll stack
        if (moveDir.magnitude == 0 || moveState == MovementState.Roll) { return; }

        moveState = MovementState.Roll;
        
        _rb.velocity = Vector3.zero;
        Vector3 dest = transform.position + (moveDir) * rollDistance;
        Vector3 rollAxis = Vector3.Cross(moveDir, playerVisualProto.up);
        
        StartCoroutine(LerpPosRoutine(dest, rollAxis));
    }
    
    private IEnumerator LerpPosRoutine(Vector3 dest, Vector3 rollAxis)
    {
        var startPos = transform.position;
        float time = 0;
        
        while (time < rollTime)
        {
            transform.position = Vector3.Lerp(startPos, dest, time / rollTime);
            time += Time.deltaTime;
            
            if(toggleProtoRoll)
                playerVisualProto.RotateAround(transform.position, rollAxis, rollAngleProto);
            
            
            yield return null;
        }
        
        transform.position = dest;
        playerVisualProto.up = Vector3.up;
        moveState = MovementState.Normal;
    }
    
    //For Sliding
    //check for velocity from rb
    //if reached max speed -> let player slide (lerp)
    private void ActionSlide(Vector3 moveDir)
    {
        if (Mathf.Abs(_rb.velocity.magnitude - maxSpeed) <= 0.1f)
        {
            //_rb.AddForce(  * moveDir);
        }
    }
}


