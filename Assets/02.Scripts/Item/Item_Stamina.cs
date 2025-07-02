using UnityEngine;

public class Item_Stamina : ItemObject
{
    public float StaminaRecoverAmount = 20f;
    protected override void ApplyItemEffect(Player player)
    {
        player.RecoverStamina(StaminaRecoverAmount);
    }
}
