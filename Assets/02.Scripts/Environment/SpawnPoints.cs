using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    private static SpawnPoints _instance;
    public static SpawnPoints Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField]
    private List<Transform> _spawnPoints;

    public Vector3 GetRandomSpawnPoint()
    {
        return _spawnPoints.Count > 0 ? _spawnPoints[Random.Range(0, _spawnPoints.Count)].position : Vector3.zero;
    }
}
