using UnityEngine;

public class Item_Score : ItemObject
{
    public int ScoreAmount = 1000;
    
    protected override void ApplyItemEffect(Player player)
    {
        ScoreManager.Instance.AddScore(ScoreAmount);
    }
}
