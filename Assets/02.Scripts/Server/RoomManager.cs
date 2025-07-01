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

    // '내가' 방에 입장하면 자동으로 호출되는 함수
    public override void OnJoinedRoom()
    {
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

    private void GeneratePlayer()
    {
        // 방에 입장 완료가되면 플레이어를 생성한다.
        // 포톤에서는 게임 오브젝트 생성후 포톤 서버에 등록까지 해야 한다.
        // 게임 오브젝트 대신 프리팹 이름이 들어간다.
        // Resources 폴더 안에 있는 프리팹을 찾아서 생성한다.
        Vector3 spawnPoint = SpawnPoints.Instance.GetRandomSpawnPoint();
        PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
    }

    private void SetRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        Debug.Log(_room.Name);
        Debug.Log(_room.PlayerCount);
        Debug.Log(_room.MaxPlayers);
    }
}
