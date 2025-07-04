using System.Collections;
using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private Dictionary<string, int> _scores = new Dictionary<string, int>();
    public Dictionary<string, int> Scores => _scores;

    private int _score = 0;
    public int Score => _score;
    private int _killCount = 0;
    public int KillCount => _killCount;
    public int KillScore = 5000;
    private int _totalScore = 0;
    public int TotalScore => _totalScore;

    public event Action OnDataChanged;

    public override void OnJoinedRoom()
    {
        Refresh();
    }

    private void Start()
    {
        // 방에 들어가면 '내 점수가 0이다' 라는 내용으로
        // 커스텀 프로퍼티를 초기화해준다.
        if(PhotonNetwork.InRoom)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["Score"] = _totalScore;

        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    public void AddKillCount()
    {
        _killCount++;
    }

    public void AddScore(int addedScore)
    {
        _score += addedScore;
        _totalScore = _killCount * KillScore + _score;

        // 프로퍼티 밸류 수정
        Refresh();
    }

    public void ResetScore()
    {
        _score = 0;
    }

    // 플레이어의 커스텀 프로퍼티가 변경되면 호출되는 콜백 함수
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable hashtable)
    {
        var roomPlayers = PhotonNetwork.PlayerList;
        foreach (Photon.Realtime.Player player in roomPlayers)
        {
            if (player.CustomProperties.ContainsKey("Score"))
            {
                _scores[$"{player.NickName}_{player.ActorNumber}"] = (int)player.CustomProperties["Score"];
            }
        }
        
        OnDataChanged?.Invoke();
    }
}
