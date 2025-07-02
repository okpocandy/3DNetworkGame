using UnityEngine;

public class Item_Score : ItemObject
{
    public int ScoreAmount = 10;
    
    protected override void ApplyItemEffect(Player player)
    {
        player.Score += ScoreAmount;
    }
}
