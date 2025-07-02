using UnityEngine;

public class Item_Health : ItemObject
{
    public float HealthRecoverAmount = 10f;
    protected override void ApplyItemEffect(Player player)
    {
        player.RecoverHealth(HealthRecoverAmount);
    }
}
