using TMPro;
using UnityEngine;

public class PlayerNicknameAbility : PlayerAbility
{
    public TextMeshProUGUI NicknameTextUI;

    private void Start()
    {
        NicknameTextUI.text = $"{_photonView.Owner.NickName}_{_photonView.Owner.ActorNumber}";

        if(_photonView.IsMine)
        {
            NicknameTextUI.color = Color.blue;
        }
        else
        {
            NicknameTextUI.color = Color.red;
        }
    }
}
