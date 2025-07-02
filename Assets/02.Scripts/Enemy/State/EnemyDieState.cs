using UnityEngine;

public class EnemyDieState : EnemyStateBase
{
    public EnemyDieState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    private float _respawnTimer = 0f;
    private float _respawnTime = 10f;

    public override void Enter()
    {
        stateMachine.Agent.isStopped = true;
        _respawnTimer = 0f;
    }

    public override void Update()
    {
        _respawnTimer += Time.deltaTime;
        if(_respawnTimer >= _respawnTime)
        {
            stateMachine.EnemyBear.Respawn();
            ChangeState(EnemyState.Patrol);
        }
    }

    public override void Exit()
    {
        stateMachine.Agent.isStopped = false;
    }
    
} 