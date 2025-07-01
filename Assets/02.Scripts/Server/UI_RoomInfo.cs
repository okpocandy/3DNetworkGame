using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UI_RoomInfo : MonoBehaviour
{
    public TextMeshProUGUI NameTextUI;
    public TextMeshProUGUI CountTextUI;

    private void Start()
    {
        RoomManager.Instance.OnRoomDataChanged += Refresh;
        Refresh();
    }
    
    private void Refresh()
    {
        Room room = RoomManager.Instance.Room;
        if(room == null) return;

        NameTextUI.text = room.Name;
        CountTextUI.text = $"{room.PlayerCount}/{room.MaxPlayers}";
    }

    public void OnclickExitButton()
    {
        Exit();
    }

    private void Exit()
    {

    }
}
