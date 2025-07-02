using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    private Animator _animator;
    public Animator MyAnimator => _animator;
    private Enemy_Bear _enemyBear;
    public Enemy_Bear EnemyBear => _enemyBear;
    
    private NavMeshAgent _agent;
    public NavMeshAgent Agent => _agent;

    [Header("State Settings")]
    public float traceDistance = 10f;
    public float traceOffDistance = 15f;
    public float attackDistance = 2f;
    public float AttackDamage = 20f;

    [Header("Debug")]
    [SerializeField] private EnemyState currentStateType = EnemyState.Patrol;

    // Properties for states to access
    public float TraceDistance => traceDistance;
    public float AttackDistance => attackDistance;
    public Transform Player { get; private set; }

    private Dictionary<EnemyState, IState> states;
    private IState currentState;

    private void Awake()
    {
        //Player = FindClosestPlayer();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _enemyBear = GetComponent<Enemy_Bear>();
        
        // Initialize states
        InitializeStates();
        
        // Set initial state
        ChangeState(EnemyState.Patrol);
    }

    private void Start()
    {
        _enemyBear.OnDie += OnDie;
    }

    private void OnDie()
    {
        ChangeState(EnemyState.Die);
    }

    private void Update()
    {
        currentState?.Update();
    }

    private void InitializeStates()
    {
        states = new Dictionary<EnemyState, IState>
        {
            { EnemyState.Patrol, new EnemyPatrolState(this) },
            { EnemyState.Trace, new EnemyTraceState(this) },
            { EnemyState.Attack, new EnemyAttackState(this) },
            { EnemyState.Die, new EnemyDieState(this) }
        };
    }

    public void ChangeState(EnemyState newState)
    {
        if (!states.ContainsKey(newState))
        {
            Debug.LogError($"State {newState} not found!");
            return;
        }

        // Exit current state
        currentState?.Exit();

        // Change to new state
        currentState = states[newState];
        currentStateType = newState;

        // Enter new state
        currentState.Enter();
    }

    // Get current state for debugging
    public EnemyState GetCurrentState()
    {
        return currentStateType;
    }

    public Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject playerObj in players)
        {
            // 죽은 플레이어는 무시
            Player player = playerObj.GetComponent<Player>();
            if (player != null && player.State == EPlayerState.Death)
                continue;

            float dist = Vector3.Distance(playerObj.transform.position, currentPos);
            if (dist < minDist)
            {
                closest = playerObj.transform;
                minDist = dist;
            }
        }
        Player = closest;

        return closest;
    }

    public void OnAttack()
    {
        EnemyAttackState attackState = states[EnemyState.Attack] as EnemyAttackState;
        attackState.IsAttackEnd = true;
    }
} 