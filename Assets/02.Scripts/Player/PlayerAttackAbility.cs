using UnityEngine;

public class PlayerAttackAbility : MonoBehaviour
{
    private Animator _animator;

    [SerializeField]
    private float _attackCooltime = 0.6f;
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
        _attackTimer += Time.deltaTime;

        if(_attackTimer < _attackCooltime)
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
