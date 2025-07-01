using Photon.Pun;
using UnityEngine;

public class PlayerAttackAbility : PlayerAbility
{
    public Collider WeaponCollider;

    private Animator _animator;

    [SerializeField]
    private float _attackTimer = 0f;

    private const string ATTACK1 = "attack1";
    private const string ATTACK2 = "attack2";
    private const string ATTACK3 = "attack3";

    private void Start()
    {
        _animator = GetComponent<Animator>();

        DeActiveCollider();
    }

    // - 위치/회전처럼 상시로 확인이 필요한 데이터 동기화: IPunObservable(OnPhotonSerializeView)
    // - '트리거/공격/피격' 처럼 간헐적으로 특정한 이벤트가 발생했을때의 변화된 데이터 동기화 : RPC
    // RPC : Remote Procedure Call
    //       ㄴ 물리적으로 떨어져 있는 다른 디바이스의 함수를 호출하는 기능
    //       ㄴ RPC 함수를 호출하면 네트워클르 통해 다른 사용자의 스크립트에서 해당 함수가 호출된다.

    private void Update()
    {
        if(!_photonView.IsMine || _owner.State == EPlayerState.Death)
        {
            return;
        }
        
        _attackTimer += Time.deltaTime;

        if(_attackTimer < (1f / _owner.Stat.AttackSpeed))
        {
            return;
        }

        if(Input.GetMouseButton(0) && _owner.TryConsumeStamina(_owner.Stat.StaminaAttackCost))
        {
            _attackTimer = 0f;
            //PlayAttackAnimation(Random.Range(1,4));
            _photonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, Random.Range(1,4));
        }
    }

    public void ActiveCollider()
    {
        WeaponCollider.enabled = true;
    }

    public void DeActiveCollider()
    {
        WeaponCollider.enabled = false;
    }

    [PunRPC]
    private void PlayAttackAnimation(int randomNumber)
    {
        _animator.SetTrigger($"attack{randomNumber}");
    }

    public void Hit(Collider other)
    {
        // 내 캐릭터가 아니면 무시
        if(_photonView.IsMine == false || _owner.State == EPlayerState.Death)
        {
            return;
        }

        // 끄지 않는다면 Hit가 여러번 호출될 수 있다.
        DeActiveCollider();

        // RPC로 호출해야지 다른 사람의 게임오브젝트들도 이 함수가 실행된다.
        // damagedObject.Damaged(_owner.Stat.Damage);

        // 데미지를 받는 오브젝트의 데미지 처리
        if(other.GetComponent<IDamaged>() == null) return;

        PhotonView otherPhotonView = other.GetComponent<PhotonView>();
        otherPhotonView.RPC(nameof(Player.Damaged), RpcTarget.All, _owner.Stat.Damage, _photonView.Owner.ActorNumber);
    }
}
