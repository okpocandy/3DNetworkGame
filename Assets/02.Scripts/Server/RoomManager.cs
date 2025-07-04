using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private Room _room;
    public Room Room => _room;

    public event Action OnRoomDataChanged;
    public event Action<string> OnPlayerEntered;
    public event Action<string> OnPlayerExited;
    public event Action<string, string> OnPlayerDeathEvent;

    public static RoomManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
    }

    bool _initialized = false;

    public override void OnJoinedRoom()
    {
        Init();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if(_initialized)
        {
            return;
        }

        _initialized = true;
        // 플레이어 생성
        GeneratePlayer();

        // 룸 설정
        SetRoom();

        OnRoomDataChanged?.Invoke();
    }

    // '나를 제외하고' 새로운 플레이어가 방에 입장하면 자동으로 호출되는 함수수
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        OnRoomDataChanged?.Invoke();
        OnPlayerEntered?.Invoke(newPlayer.NickName + "_" + newPlayer.ActorNumber);
    }

    // '나를 제외하고' 플레이어가 방에서 퇴장하면 자동으로 호출되는 함수
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        OnRoomDataChanged?.Invoke();
        OnPlayerExited?.Invoke(otherPlayer.NickName + "_" + otherPlayer.ActorNumber);
    }

    public void OnPlayerDeath(int actorNumber, int killerActorNumber)
    {
        // actorNumber가 killerActorNumber에 의해 죽었다.
        string diedNickname = _room.GetPlayer(actorNumber).NickName;
        string killerNickname = _room.GetPlayer(killerActorNumber).NickName;
        if(diedNickname != null)
        {
            diedNickname = diedNickname + "_" + actorNumber;
        }
        if(killerNickname == null)
        {
            killerNickname = "데드존";
        }
        else
        {
            killerNickname = killerNickname + "_" + killerActorNumber;
        }

        OnPlayerDeathEvent?.Invoke(diedNickname, killerNickname);
    }

    private void GeneratePlayer()
    {
        // 방에 입장 완료가되면 플레이어를 생성한다.
        // 포톤에서는 게임 오브젝트 생성후 포톤 서버에 등록까지 해야 한다.
        // 게임 오브젝트 대신 프리팹 이름이 들어간다.
        // Resources 폴더 안에 있는 프리팹을 찾아서 생성한다.
        Vector3 spawnPoint = SpawnPoints.Instance.GetRandomSpawnPoint();
        PhotonNetwork.Instantiate($"Player{LobbyScene.SelectedCharacterType}", spawnPoint, Quaternion.identity);
    }

    private void SetRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        Debug.Log(_room.Name);
        Debug.Log(_room.PlayerCount);
        Debug.Log(_room.MaxPlayers);
    }
}
