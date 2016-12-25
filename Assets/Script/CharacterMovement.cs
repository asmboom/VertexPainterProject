﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

    public float paramaterSet=7; // 애니메이터용 최대속도랑 일치시켜주면됨
    Animator animator;// 
    new Rigidbody rigidbody; 
    //Vector3 moveDir
    [Header("Movement")]
    [SerializeField]float speed; // 현재속도
    [SerializeField]float maxSpeed; // 최대속도
    [SerializeField]float accelator; // 가속도
    Vector3 moveDir; // 이동방향
    [SerializeField]
    float jumpPowerDefault=5.0f; // 기본점프강도
    [SerializeField] 
    bool usingAnimatorMoving=true; // 애니메이션의 deltaPosition의 이동시 이동하려면 true
    [SerializeField]
    bool usingPositionMoving = false; // position으로 이동하려면 true / false 일 시 velocity로 이동
    float velocityY;
    [SerializeField]
    bool fixPosition;

    [Header("Rotating")]
    Quaternion rotationDest; // 회전하고자 하는 목표rotation
    [SerializeField]
    float rotationSpeed; // 회전속도
   
    float rotatingAmount; // animator용 회전량
    [Header("GroundCheck")]
    [SerializeField]
    float groundCheckAmount=0.3f; // 땅 체크시 y축 offset
    public bool isOnGround; 
    [SerializeField]
    public Vector3 groundNormal; // 현재 땅의 노말
    public Transform groundTransform; // 현재 땅의 트랜스폼
    public Transform lastGround; // 이전 땅의 트랜스폼
    [SerializeField]
    UnityEvent groundEvents; // 땅에 닿았을 시 이벤트
    [SerializeField]
    UnityEvent airEvents; // 공중에 떴을 시 이벤트
    void Start () {
        rigidbody = GetComponent<Rigidbody>();
    }
    void OnGround()
    {
        groundEvents.Invoke();
    }
    void OnAir()
    {
        airEvents.Invoke();
    }
    void OnAnimatorMove()
    {
        if (!usingAnimatorMoving)
            return;
        if (isOnGround&& animator.deltaPosition.magnitude > 0.001f)
        {
            UpdateMovement();
        }
    }

    void SetupAnimator()
    {
        animator.SetFloat("speed", speed/paramaterSet,0.1f,Time.deltaTime);
        animator.SetFloat("turn", Mathf.Deg2Rad*rotatingAmount, 0.1f, Time.deltaTime/3);
        animator.SetBool("isOnGround", isOnGround);
        if (!isOnGround)
            animator.SetFloat("velocityY", velocityY);
    }
    void UpdateMovement()
    {
        if (fixPosition == true)
            return;
        if (IsMoving()&&!usingPositionMoving)
        {
            // Velocity를 이용해서 이동시킴
            Vector3 v = rigidbody.velocity;
            v = moveDir * speed;
            v.y = rigidbody.velocity.y;
            rigidbody.velocity = v;
        }
        else
        {
            // position을 이용해서 이동시킴
            rigidbody.MovePosition(rigidbody.position+moveDir * speed * Time.deltaTime);
        }
    }
    public void Move(Vector3 _worldDirection, float _maxSpeed=3.0f,float _accel=0.1f,float _startSpeed=-1.0f)
    {
        if (_worldDirection.magnitude == 0)
        {
            Stop();
            return;
        }
        moveDir = _worldDirection.normalized;
        if(isOnGround)
            moveDir = Vector3.ProjectOnPlane(moveDir, groundNormal);// 바닥의 경사에따라 moveDir수정
        if(_startSpeed!=-1.0f)
            speed = _startSpeed;
        maxSpeed = _maxSpeed;
        accelator = _accel;
    }
    public void RotateTo(Quaternion _rotateTo, float _speed = 20.0f)
    {
        rotationDest = _rotateTo;
        rotationSpeed = _speed;
        Vector3 targetForward = rotationDest * Vector3.forward;
        rotatingAmount = Quaternion.Angle(transform.rotation, _rotateTo)*((Vector3.Dot(transform.up,Vector3.Cross(transform.forward,targetForward))<0)?-1:1);
    }
    public void RotateTo(Vector3 _forwardVector, float _speed = 20.0f)
    {
        RotateTo(Quaternion.LookRotation(_forwardVector), _speed);
    }
    void UpdateRotation()
    {
        if (rotationSpeed == 0.0f)
            return;
        float rSpeed = (isOnGround) ? rotationSpeed : rotationSpeed / 10;
        if (rSpeed > 0)
        {
            float rAngle = Quaternion.Angle(transform.rotation, rotationDest);// 남은 회전량
            if (Quaternion.Angle(transform.rotation, rotationDest) < rSpeed)
            {
                transform.rotation = rotationDest;
                StopRotating();
                return;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation,rotationDest,rSpeed);
        }
    }
    public void Stop()
    {
        /*if (accelator == 0.0f)
        {
            return;
        }*/
        maxSpeed = 0.0f;
        moveDir = Vector3.zero;
        speed = 0.0f;
        accelator = 0.0f;
        //SetupAnimator();
    }
    public bool IsMoving()
    {
        return (speed > 0);
    }
    public void StopSmooth(float _accel=0.2f)
    {
        Move(moveDir, maxSpeed, -_accel);
    }
    public void StopRotating()
    {
        rotationSpeed = 0.0f;
        rotatingAmount = 0.0f;
        //SetupAnimator();
    }
    public void GroundCheck()
    {
        RaycastHit hitInfo;
        float groundDistance = (!isOnGround) ? 0.1f : groundCheckAmount;
        if (Physics.Raycast(rigidbody.position + transform.up * 0.06f, transform.up * -1, out hitInfo, groundCheckAmount,LayerMask.GetMask("ground")))
        {
            if (!isOnGround && rigidbody.velocity.y > 0)// 점프중일때는 무시
                return;
            if (!isOnGround) OnGround();
            isOnGround = true;
        }
        else
        {
            if (isOnGround) OnAir();
            isOnGround = false;
        }
        velocityY = rigidbody.velocity.y;
    }
    public void Jump(float _power=0.0f)
    {
        if (isOnGround)
        {
            if (_power == 0.0f)
                _power = jumpPowerDefault;
            rigidbody.velocity +=transform.up*_power;
            rigidbody.useGravity = true;
            isOnGround = false;
        }
    }
    void Accelation()
    {
        if (accelator != 0.0f)
        {
            if (speed > maxSpeed)
            {
                speed -= accelator;
            }
            else speed += accelator;
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            else if (speed <= 0)
            {
                Stop();
                return;
            }
        }
    }
	void FixedUpdate ()
    {
        Accelation();
        //SetupAnimator();
        UpdateRotation();
        if (!usingAnimatorMoving) UpdateMovement();
        GroundCheck();
    }
}
