using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemySpawn : MonoBehaviourPunCallbacks
{
    private static EnemySpawn _instance;
    public static EnemySpawn Instance => _instance;

    public List<Transform> SpawnPoints;

    private void Awake()
    {
        _instance = this;
    }

    public override void OnJoinedRoom()
    {
        // 내가 방장이라면 적 생성
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Bear", SpawnPoints[0].position, Quaternion.identity);
        }
    }

}
