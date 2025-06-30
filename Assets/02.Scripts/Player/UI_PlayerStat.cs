using RainbowArt.CleanFlatUI;
using UnityEngine;

public class UI_PlayerStat : MonoBehaviour
{
    private Player _player;

    public ProgressBar HealthSlider;
    public ProgressBar StatminaSlider;

    public void SetPlayer(Player player)
    {
        _player = player;
    }
    
    
    private void Update()
    {
        if (_player == null)
        {
            return;
        }
        
        StatminaSlider.CurrentValue = _player.Stat.Stamina;
        HealthSlider.CurrentValue = _player.Stat.Health;
    }
}
