using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using RainbowArt.CleanFlatUI;
using Photon.Pun;
using System.Collections;

public enum EPlayerState
{
    Live,
    Death,
}

[RequireComponent(typeof(PlayerMoveAbility))]
public class Player : MonoBehaviour, IPunObservable, IDamaged
{
    public PlayerStat Stat;
    public EPlayerState State = EPlayerState.Live;


    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();

    private PlayerMoveAbility _myMoveAbility;
    private CharacterController _myCharacterController;
    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;
    private Animator _myAnimator;

    public ProgressBar StaminaBar;
    public ProgressBar HealthBar;

    public GameObject MiniMapEnemyIcon;
    public GameObject MiniMapPlayerIcon;

    public ItemData ItemData;

    



    private void Awake()
    {
        Stat.Health = Stat.MaxHealth;
        Stat.Stamina = Stat.MaxStamina;
        _myMoveAbility = GetComponent<PlayerMoveAbility>();
        _myCharacterController = GetComponent<CharacterController>();
        _photonView = GetComponent<PhotonView>();
        _myAnimator = GetComponent<Animator>();

        State = EPlayerState.Live;
        ItemData = new ItemData();

    }

    private void Start()
    {
        StaminaBar.MaxValue = Stat.MaxStamina;
        StaminaBar.CurrentValue = Stat.Stamina;
        HealthBar.MaxValue = Stat.MaxHealth;
        HealthBar.CurrentValue = Stat.Health;


        if(_photonView.IsMine)
        {
            // 미니맵 카메라 추적
            var miniMapCamera = GameObject.FindWithTag("MinimapCamera");
            if(miniMapCamera != null)
            {
                miniMapCamera.GetComponent<CopyPosition>().SetTarget(transform);
            }
            
            MiniMapPlayerIcon.SetActive(true);
            MiniMapEnemyIcon.SetActive(false);

            // 플레이어 스탯 UI 설정
            var uiPlayerStat = GameObject.FindAnyObjectByType<UI_PlayerStat>();
            if(uiPlayerStat != null)
            {
                uiPlayerStat.SetPlayer(this);
            }
        }
        else
        {
            MiniMapPlayerIcon.SetActive(false);
            MiniMapEnemyIcon.SetActive(true);
        }
    }

    private void Update()
    {
        // 달리고 있거나, 점프를 하고 있거나, 공격을 하고 있거나
        if(_myMoveAbility.IsDashing || _myCharacterController.isGrounded == false)
        {
            return;
        }
        RecoverStamina(Stat.StaminaRecoverySpeed * Time.deltaTime);
    }

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        // 게으른 초기화/로딩 -> 처음에 곧바로 초기화/로딩을 하는게 아니라
        //                    필요할때만 하는.. 뒤로 미루는 기법
        ability = GetComponent<T>();

        if (ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;

            return ability as T;
        }
        
        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");
    }

    public bool TryConsumeStamina(float amount)
    {
        if(Stat.Stamina < 0)
        {
            throw new Exception("스테미너 감소량이 0보다 작을 수 없다");
        }
        if(Stat.Stamina - amount < 0)
        {
            Debug.Log("스테미나 부족");
            return false;
        }
        Stat.Stamina -= amount;

        // 스테미나 바 업데이트
        StaminaBar.CurrentValue = Stat.Stamina;

        return true;
    }

    public bool TryConsumeStaminaPerSecond(float amount)
    {
        if (amount < 0f)
            throw new Exception("스테미너 감소량이 0보다 작을 수 없다");

        float cost = amount * Time.deltaTime;
        if (Stat.Stamina < cost)
        {
            Debug.Log("스태미나 부족");
            return false;
        }

        Stat.Stamina -= cost;

        // 스테미나 바 업데이트
        StaminaBar.CurrentValue = Stat.Stamina;
        
        return true;
    }


    public void RecoverStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.Log("스테미너 회복량이 0보다 작을 수 없다.");
        }
        Stat.Stamina = math.min(Stat.Stamina + amount, Stat.MaxStamina);
        StaminaBar.CurrentValue = Stat.Stamina;
    }

    public void RecoverHealth(float amount)
    {
        if(amount < 0)
        {
            throw new Exception("체력 회복량이 0보다 작을 수 없다.");
        }
        Stat.Health = math.min(Stat.Health + amount, Stat.MaxHealth);
        HealthBar.CurrentValue = Stat.Health;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting && _photonView.IsMine)
        {
            // 데이터를 전송한느 상황 -> 데이터를 보내주면 되고 (보내는 순서대로)
            stream.SendNext(Stat.Stamina);
            stream.SendNext(Stat.Health);
        }
        else if(stream.IsReading && !_photonView.IsMine)
        {
            // 데이터를 수신하는 상황 - 받은 데이터를 세텅하면 된다. (받을 수 있다.)
            Stat.Stamina = (float)stream.ReceiveNext();
            Stat.Health = (float)stream.ReceiveNext();
            StaminaBar.CurrentValue = Stat.Stamina;
            HealthBar.CurrentValue = Stat.Health;
        }
    }

    [PunRPC]
    public void Damaged(float damage, int actorNumber)
    {
        if(State == EPlayerState.Death)
        {
            return;
        }

        Stat.Health = Mathf.Max(0, Stat.Health - damage);
        HealthBar.CurrentValue = Stat.Health;

        Debug.Log($"남은 체력 : {Stat.Health}");

        if(Stat.Health <= 0)
        {
            // 플레이어 죽음.
            // 사망 애니메이션
            // 움직이지 못한다.
            // 5초 후에 체력과 스테미너 회복된 상태에서 랜덤한 위치에 리스폰

            State = EPlayerState.Death;

            StartCoroutine(Death_Coroutine());



            if (_photonView.IsMine)
            {

                if(actorNumber == 999)
                {
                    return;
                }

                RoomManager.Instance.OnPlayerDeath(_photonView.Owner.ActorNumber, actorNumber);

                // 날 죽인 사람 킬 카운트 증가
                // 점수 증가
                var killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
                _photonView.RPC(nameof(AddKillCount), killerPlayer);

                int score = ScoreManager.Instance.Score/2;
                MakeItems((ScoreManager.Instance.Score - score)/1000);
                _photonView.RPC(nameof(AddScore), killerPlayer, score);
                ScoreManager.Instance.ResetScore();
                ScoreManager.Instance.AddScore(0);
            }
        }
        else
        {
            // RPC로 호출 X
            GetAbility<PlayerShakeAbility>().Shake();
        }

        if(_photonView.IsMine)
        {
            Debug.Log("카메라 흔들기");
            CameraShaking_Noise.Instance.Shake(3f, 3f, 0.5f);
        }
    }

    private IEnumerator Death_Coroutine()
    {
        _myCharacterController.enabled = false;

        _photonView.RPC(nameof(DeathAnimation), RpcTarget.All);

        yield return new WaitForSeconds(5f);

        Respawn();
    }

    [PunRPC]
    private void DeathAnimation()
    {
        _myAnimator.SetTrigger("Death");
    }

    private void Respawn()
    {
        Stat.Health = Stat.MaxHealth;
        Stat.Stamina = Stat.MaxStamina;

        StaminaBar.CurrentValue = Stat.Stamina;
        HealthBar.CurrentValue = Stat.Health;

        if(_photonView.IsMine)
        {
            // 위치 변경
            transform.position = SpawnPoints.Instance.GetRandomSpawnPoint();
        }
        
        Debug.Log("부활");

        State = EPlayerState.Live;
        _myCharacterController.enabled = true;
        _myAnimator.SetTrigger("Alive");
    }

    private void MakeItems(int count)
    {
        for (int i=0; i<count; i++)
        {
            float randomNumber = UnityEngine.Random.value;
            if(randomNumber < ItemData.HealthItemProbability)
            {
                ItemObjectFactory.Instance.RequestCreate(EItemType.Health, transform.position + new Vector3(0, 1, 0));
            }
            else if(randomNumber < ItemData.HealthItemProbability + ItemData.StaminaItemProbability)
            {
                ItemObjectFactory.Instance.RequestCreate(EItemType.Stamina, transform.position + new Vector3(0, 1, 0));
            }
            else
            {
                ItemObjectFactory.Instance.RequestCreate(EItemType.Score, transform.position + new Vector3(0, 1, 0));
            }
        }
    }

    [PunRPC]
    public void AddKillCount()
    {
        ScoreManager.Instance.AddKillCount();
    }

    [PunRPC]
    public void AddScore(int addedScore)
    {
        ScoreManager.Instance.AddScore(addedScore);
    }

    [PunRPC]
    public void ResetScore()
    {
        ScoreManager.Instance.ResetScore();
    }
}
