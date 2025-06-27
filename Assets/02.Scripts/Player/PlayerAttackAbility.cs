using UnityEngine;

public class PlayerAttackAbility : PlayerAbility
{

    private Animator _animator;

    [SerializeField]
    private float _attackTimer = 0f;

    private const string ATTACK1 = "attack1";
    private const string ATTACK2 = "attack2";
    private const string ATTACK3 = "attack3";

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(!_photonView.IsMine)
        {
            return;
        }
        
        _attackTimer += Time.deltaTime;

        if(_attackTimer < (1f / _owner.Stat.AttackSpeed))
        {
            return;
        }

        if(Input.GetMouseButton(0))
        {
            _attackTimer = 0f;
            _animator.SetTrigger($"attack{Random.Range(1, 4)}");
        }
    }
}
