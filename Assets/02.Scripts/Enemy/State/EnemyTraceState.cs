using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class EnemyTraceState : EnemyStateBase
{
    public EnemyTraceState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    private float _newTargetTimer = 0f;
    private float _newTargetFindInterval = 3f;

    public override void Enter()
    {
        Debug.Log("Entering Trace State");
        // 추적 시작 시 초기화 코드
        stateMachine.Agent.SetDestination(stateMachine.Player.position);
        stateMachine.MyAnimator.ResetTrigger("Walk");
        stateMachine.EnemyBear.PhotonView.RPC(nameof(Enemy_Bear.PlayRunAnimation), RpcTarget.All);

        _newTargetTimer = 0f;

    }

    // 플레이어가 추적이 풀릴 때까지 목표 플레이어를 변경하지 안는다.
    public override void Update()
    {
        Debug.Log("Tracing player...");
        // 플레이어 추적 이동 코드

        // 일정 시간마다 가장 목표로 바꾼다.
        _newTargetTimer += Time.deltaTime;
        if(_newTargetTimer >= _newTargetFindInterval)
        {
            var newTarget = stateMachine.FindClosestPlayer();
            _newTargetTimer = 0f;
        }

        stateMachine.Agent.SetDestination(stateMachine.Player.position);

        // 플레이어가 죽으면 순찰로 전환
        Player targetPlayer = stateMachine.Player.GetComponent<Player>();
        if (targetPlayer.State == EPlayerState.Death)
        {
            ChangeState(EnemyState.Patrol);
        }

        float distance = Vector3.Distance(enemyTransform.position, stateMachine.Player.position);
        if (distance > stateMachine.TraceDistance)
        {
            ChangeState(EnemyState.Patrol);
        }
        else if (distance <= stateMachine.AttackDistance)
        {
            ChangeState(EnemyState.Attack);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Trace State");
        // 추적 종료 시 정리 코드
    }
} 