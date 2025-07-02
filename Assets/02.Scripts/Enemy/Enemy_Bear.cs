using UnityEngine;
using Photon.Pun;
using RainbowArt.CleanFlatUI;
using System.Collections;
using System;

public class Enemy_Bear : MonoBehaviourPun, IDamaged
{
    public float MaxHealth = 100f;
    public float Health = 100f;
    public ProgressBar HealthBar;


    public float AttackDamage = 20f;
    public float AttackRange = 10f;
    public EPlayerState State = EPlayerState.Live;

    private Animator _animator;
    
    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;

    public event Action OnDie;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _photonView = GetComponent<PhotonView>();
        State = EPlayerState.Live;
        Health = MaxHealth;
        HealthBar.CurrentValue = Health;
    }

    public void BearAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, AttackRange);
        foreach(Collider collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                Player player = collider.GetComponent<Player>();
                //player.PhotonView.RPC(nameof(Player.Damaged), RpcTarget.All, AttackDamage, photonView.Owner.ActorNumber);
                player.Damaged(AttackDamage, PhotonView.Owner.ActorNumber);
            }
        }
    }

    [PunRPC]
    public void PlayAttackAnimation()
    {
        _animator.SetTrigger("Attack");
    }

    [PunRPC]
    public void PlayWalkAnimation()
    {
        _animator.SetTrigger("Walk");
    }

    [PunRPC]
    public void PlayRunAnimation()
    {
        _animator.SetTrigger("Run");
    }

    [PunRPC]
    public void PlayDieAnimation()
    {
        _animator.SetTrigger("Die");
    }

    [PunRPC]
    public void Damaged(float damage, int actorNumber)
    {
        if(State == EPlayerState.Death)
        {
            return;
        }

        Health = Mathf.Max(0, Health - damage);
        HealthBar.CurrentValue = Health;

        if(Health <= 0)
        {
            State = EPlayerState.Death;
            OnDie?.Invoke();
            _photonView.RPC(nameof(PlayDieAnimation), RpcTarget.All);
        }
        
    }

    public void Respawn()
    {
        Health = MaxHealth;
        State = EPlayerState.Live;
        HealthBar.CurrentValue = Health;

        transform.position = EnemySpawn.Instance.SpawnPoints[0].position;
    }
}
