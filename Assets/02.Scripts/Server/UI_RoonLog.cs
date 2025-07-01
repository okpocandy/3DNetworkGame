using TMPro;
using UnityEngine;

public class UI_RoonLog : MonoBehaviour
{
    public TextMeshProUGUI LogTextUI;

    private string _logMessage = "방베 입장했습니다.";

    private const string LIME = "#00ff00ff";
    private const string AQUA = "#00ffffff";
    private const string RED = "#ff0000ff";

    private void Start()
    {
        RoomManager.Instance.OnPlayerEntered += PlayerEnterLog;
        RoomManager.Instance.OnPlayerExited += PlayerExitLog;

        Refresh();
    }
    private void Refresh()
    {
        LogTextUI.text = _logMessage;
    }

    public void PlayerEnterLog(string playerName)
    {
        _logMessage += $"\n<color={LIME}>{playerName}</color>님이 <color={AQUA}>입장</color>하였습니다.";
        Refresh();
    }

    public void PlayerExitLog(string playerName)
    {
        _logMessage += $"\n<color={LIME}>{playerName}</color>님이 <color={RED}>퇴장</color>하였습니다.";
        Refresh();
    }


}
