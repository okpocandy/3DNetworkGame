using Photon.Pun;
using UnityEngine;

public class EnemyPatrolState : EnemyStateBase
{
    public EnemyPatrolState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    private int currentPatrolIndex = 0;

    public override void Enter()
    {
        Debug.Log("Entering Patrol State");
        // 순찰 시작 시 초기화 코드
        stateMachine.FindClosestPlayer();
        stateMachine.Agent.SetDestination(EnemySpawn.Instance.SpawnPoints[currentPatrolIndex].position);
        stateMachine.EnemyBear.PhotonView.RPC(nameof(Enemy_Bear.PlayWalkAnimation), RpcTarget.All);
    }

    public override void Update()
    {
        Debug.Log("Patrolling...");
        // 순찰 경로 이동 코드
        if(stateMachine.Agent.remainingDistance <= 1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % EnemySpawn.Instance.SpawnPoints.Count;
            stateMachine.Agent.SetDestination(EnemySpawn.Instance.SpawnPoints[currentPatrolIndex].position);
        }
        
        // 가장 가까이 있는 플레이어가 범위내에 있으면 그 플레이어를 추적 상태로 변경
        float distance = Vector3.Distance(enemyTransform.position, stateMachine.FindClosestPlayer().position);
        if (distance <= stateMachine.TraceDistance)
        {
            ChangeState(EnemyState.Trace);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Patrol State");
        // 순찰 종료 시 정리 코드
    }
} 