using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovementController : MonoBehaviour
{
    //max speed 
    //acceleration - how fast to reach max speed
    //max accel force - max force to be applied to reach accel

    //shouldn't be lower than 1 unless want to slow down accel force
    
    public bool canMove = true;
    public bool allowAccelTamper = true;
    
    [SerializeField] private float accelRate = 1;
    [SerializeField] private float accelForce;
    [SerializeField] private float maxSpeed;

    private Rigidbody _rb;


    [Header("Roll Attributes")] 
    [SerializeField]  private KeyCode rollKey;
    [Space]
    [SerializeField] private float rollDistance;
    [SerializeField] private float rollTime;
    [SerializeField] private float routineInterval;

    [Header("Slide Attributes")] [SerializeField]
    private KeyCode slideKey;
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
        Vector3 moveDir = new Vector3(_horiz, 0 , _vert).normalized;
        
        
        //if(Input.GetKeyDown(rollKey)) ActionRoll(moveDir);
        if(canMove) UpdateStrafe(moveDir);
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
    
    //For Rolling
    //Reset velocity from rigidbody
    //lerp to designated position = roll distance * input dir vector
    // private void ActionRoll(Vector3 moveDir)
    // {
    //     if (moveDir.magnitude == 0)
    //     {
    //         Debug.Log("no roll");
    //         return;
    //     }
    //     Debug.Log("yes roll");
    //     
    //     _rb.velocity = Vector3.zero;
    //     Vector3 dest = transform.position + (moveDir) * rollDistance;
    //     StartCoroutine(LerpPosRoutine(dest));
    // }
    //
    // private IEnumerator LerpPosRoutine(Vector3 dest)
    // {
    //     var startPos = transform.position;
    //     float x = Time.time;
    //     
    //     while (x < rollTime)
    //     {
    //         canMove = false;
    //         Vector3.Lerp(startPos, dest, x / rollTime);
    //         x += Time.deltaTime;
    //
    //         yield return null;
    //     }
    //
    //     canMove = true;
    //     transform.position = dest;
    // }
    
    //For Sliding
    //check for velocity from rb
    //if reached max speed -> let player slide (add force)
    
    private void ActionSlide(Vector3 moveDir)
    {
        if (Mathf.Abs(_rb.velocity.magnitude - maxSpeed) <= 0.1f)
        {
            //_rb.AddForce(  * moveDir);
        }
    }
}


