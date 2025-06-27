using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    public float MoveSpeed = 7f;
    public float JumpForce = 2.5f;
    public float RoationSpeed = 300f;
    public float AttackSpeed = 1f;  // 초당 1번 공격할 수 있다.

    // 스테미너
    public float MaxStamina = 100f;
    public float StaminaRecoverySpeed = 20f;
    public float StaminaRunCost = 10f;
    public float StaminaJumpCost = 10f;
    public float StaminaAttackCost = 20f;

    // 체력
    public float MaxHealth = 100f;
}
