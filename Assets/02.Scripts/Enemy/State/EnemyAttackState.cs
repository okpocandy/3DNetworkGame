using Photon.Pun;
using UnityEngine;

public class EnemyAttackState : EnemyStateBase
{
    public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public bool IsAttackEnd = false;



    public override void Enter()
    {
        Debug.Log("Entering Attack State");
        // 공격 시작 시 초기화 코드
        stateMachine.Agent.isStopped = true;
        stateMachine.EnemyBear.PhotonView.RPC(nameof(Enemy_Bear.PlayAttackAnimation), RpcTarget.All);
        IsAttackEnd = false;
    }

    public override void Update()
    {
        Debug.Log("Attacking player!");
       
       // 공격 애니메이션이 끝나면 
        if(IsAttackEnd)
        {
            // 공격 1회 후 Trace로 전환
            ChangeState(EnemyState.Trace);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Attack State");
        // 공격 종료 시 정리 코드
        stateMachine.Agent.isStopped = false;
    }
} 