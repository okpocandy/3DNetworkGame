using System;
using Photon.Pun;
using UnityEngine;

public class PlayerMoveAbility : PlayerAbility
{

    private CharacterController _characterController;
    private Animator _animator;


    // 애니메이터터
    private const string IS_MOVE = "isMove";
    private const string HORIZONTAL = "h";
    private const string VERTICAL = "v";
    private const string JUMP = "jump";
    private const string IS_GROUNDED = "isGrounded";
    private const string IS_DASHING = "isDashing";

    // 중력
    [SerializeField]
    private float _gravity = -9.81f;
    private float _yVelocity = 0f;
    
    
    [SerializeField]
    private float _maxFallSpeed = -2f; // 최대 낙하 속도 제한'

    private Vector3 _receivedPosition = Vector3.zero;
    private Quaternion _receivedRotation = Quaternion.identity;
    private float _lerpSpeed = 20f;

    // 대쉬쉬
    public bool IsDashing { get; private set;}
    private float _dashMultiplier = 2f;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(!_photonView.IsMine)
        {
            //transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * _lerpSpeed);
            //transform.rotation = Quaternion.Slerp(transform.rotation, _receivedRotation, Time.deltaTime * _lerpSpeed);
            return;
        }

        if(_owner.State == EPlayerState.Death)
        {
            return;
        }

        // 대쉬
        Dash();

        Move();

    }

    private void Dash()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(_owner.TryConsumeStaminaPerSecond(_owner.Stat.StaminaRunCost))
            {
                IsDashing = true;
                _owner.Stat.MoveSpeed *= _dashMultiplier;

                _animator.SetBool(IS_DASHING, true);
            }
        }

        if (IsDashing)
        {
            if (!Input.GetKey(KeyCode.LeftShift) || !_owner.TryConsumeStaminaPerSecond(_owner.Stat.StaminaRunCost))
            {
                // 대쉬 중단 조건: 키를 뗐거나, 스태미나 부족
                IsDashing = false;
                _owner.Stat.MoveSpeed /= _dashMultiplier;

                _animator.SetBool(IS_DASHING, false);
            }
        }
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = (transform.right * h + transform.forward * v).normalized;
        dir = dir.normalized;

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded)
        {
            if(_owner.TryConsumeStamina(_owner.Stat.StaminaJumpCost))
            {
                _yVelocity = _owner.Stat.JumpForce;
                _animator.SetTrigger(JUMP);
            }
            else
            {
                Debug.Log("점프 스태미나 부족");
            }
            
        }

        _yVelocity += _gravity * Time.deltaTime;
        _yVelocity = Mathf.Max(_yVelocity, _maxFallSpeed); // 최대 낙하 속도 제한
        dir.y = _yVelocity;

        _characterController.Move(dir * _owner.Stat.MoveSpeed * Time.deltaTime);

        if (Mathf.Approximately(dir.x, 0) && Mathf.Approximately(dir.z, 0))
        {
            _animator.SetBool(IS_MOVE, false);
        }
        else
        {
            _animator.SetBool(IS_MOVE, true);
        }
        _animator.SetFloat(HORIZONTAL, h);
        _animator.SetFloat(VERTICAL, v);
        _animator.SetBool(IS_GROUNDED, _characterController.isGrounded);
    }

    // 데이터 동기화를 위한 데이터 전송 및 수신 기능
    // stream : 서버에서 주고받을 데이터가 담겨있는 변수
    // info   : 송수신 성공/실패 여부에 대한 로그
    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting && _photonView.IsMine)
        {
            // 데이터를 전송한느 상황 -> 데이터를 보내주면 되고 (보내는 순서대로)
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if(stream.IsReading && !_photonView.IsMine)
        {
            // 데이터를 수신하는 상황 - 받은 데이터를 세텅하면 된다. (받을 수 있다.)
            _receivedPosition = (Vector3)stream.ReceiveNext();
            _receivedRotation = (Quaternion)stream.ReceiveNext();

        }
    }
    */
}
