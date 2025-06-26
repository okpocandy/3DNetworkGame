using System;
using UnityEngine;

public class PlayerMoveAbility : MonoBehaviour
{
    public float MoveSpeed = 10f;
    private CharacterController _characterController;
    private Animator _animator;

    // 애니메이터터
    private const string IS_MOVE = "isMove";
    private const string HORIZONTAL = "h";
    private const string VERTICAL = "v";
    private const string JUMP = "jump";
    private const string IS_GROUNDED = "isGrounded";

    // 중력
    [SerializeField]
    private float _gravity = -9.81f;
    private float _yVelocity = 0f;
    
    // 점프프
    [SerializeField]
    private float _jumpForce = 2f;
    
    [SerializeField]
    private float _maxFallSpeed = -2f; // 최대 낙하 속도 제한'
    private bool _isGrounded = true;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = (transform.right * h + transform.forward * v).normalized;
        dir = dir.normalized;

        if (Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded)
        {
            _yVelocity = _jumpForce;
            _animator.SetTrigger(JUMP);
            _isGrounded = false;
            _animator.SetBool(IS_GROUNDED, false);
        }

        _yVelocity += _gravity * Time.deltaTime;
        _yVelocity = Mathf.Max(_yVelocity, _maxFallSpeed); // 최대 낙하 속도 제한
        dir.y = _yVelocity;

        if(_characterController.isGrounded)
        {
            _isGrounded = true;
            _animator.SetBool(IS_GROUNDED, true);
        }

        _characterController.Move(dir * MoveSpeed * Time.deltaTime);

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
    }
}
