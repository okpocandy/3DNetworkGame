using UnityEngine;

public abstract class EnemyStateBase : IState
{
    protected EnemyStateMachine stateMachine;
    protected Transform player;
    protected Transform enemyTransform;

    public EnemyStateBase(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = stateMachine.Player;
        this.enemyTransform = stateMachine.transform;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    protected void ChangeState(EnemyState newState)
    {
        stateMachine.ChangeState(newState);
    }
} 