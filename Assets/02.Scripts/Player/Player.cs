using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using RainbowArt.CleanFlatUI;
using Photon.Pun;

public class Player : MonoBehaviour, IPunObservable
{
    public PlayerStat Stat;

    private float _currentHealth;
    [SerializeField]
    private float _currentStamina;

    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();

    private PlayerMoveAbility _myMoveAbility;
    private CharacterController _myCharacterController;
    private PhotonView _photonView;

    public ProgressBar StaminaBar;
    public ProgressBar HealthBar;

    public GameObject MiniMapEnemyIcon;
    public GameObject MiniMapPlayerIcon;

    



    private void Awake()
    {
        _currentHealth = Stat.MaxHealth;
        _currentStamina = Stat.MaxStamina;
        _myMoveAbility = GetComponent<PlayerMoveAbility>();
        _myCharacterController = GetComponent<CharacterController>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        StaminaBar.MaxValue = Stat.MaxStamina;
        StaminaBar.CurrentValue = _currentStamina;
        HealthBar.MaxValue = Stat.MaxHealth;
        HealthBar.CurrentValue = _currentHealth;


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
        if(_currentStamina < 0)
        {
            throw new Exception("스테미너 감소량이 0보다 작을 수 없다");
        }
        if(_currentStamina - amount < 0)
        {
            Debug.Log("스테미나 부족");
            return false;
        }
        _currentStamina -= amount;

        // 스테미나 바 업데이트
        StaminaBar.CurrentValue = _currentStamina;

        return true;
    }

    public bool TryConsumeStaminaPerSecond(float amount)
    {
        if (amount < 0f)
            throw new Exception("스테미너 감소량이 0보다 작을 수 없다");

        float cost = amount * Time.deltaTime;
        if (_currentStamina < cost)
        {
            Debug.Log("스태미나 부족");
            return false;
        }

        _currentStamina -= cost;

        // 스테미나 바 업데이트
        StaminaBar.CurrentValue = _currentStamina;
        
        return true;
    }


    public void RecoverStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.Log("스테미너 회복량이 0보다 작을 수 없다.");
        }
        _currentStamina = math.min(_currentStamina + amount, Stat.MaxStamina);
        StaminaBar.CurrentValue = _currentStamina;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting && _photonView.IsMine)
        {
            // 데이터를 전송한느 상황 -> 데이터를 보내주면 되고 (보내는 순서대로)
            stream.SendNext(_currentStamina);
            stream.SendNext(_currentHealth);
        }
        else if(stream.IsReading && !_photonView.IsMine)
        {
            // 데이터를 수신하는 상황 - 받은 데이터를 세텅하면 된다. (받을 수 있다.)
            _currentStamina = (float)stream.ReceiveNext();
            _currentHealth = (float)stream.ReceiveNext();
            StaminaBar.CurrentValue = _currentStamina;
            HealthBar.CurrentValue = _currentHealth;
        }
    }
}
