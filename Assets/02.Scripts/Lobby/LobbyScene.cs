using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public enum ECharacterType
{
    Male,
    Female,
}

public class LobbyScene : MonoBehaviour
{
    public TMP_InputField NicknameInputField;
    public TMP_InputField RoomNameInputField;

    public GameObject MaleCharacter;
    public GameObject FemaleCharacter; 

    public static ECharacterType SelectedCharacterType = ECharacterType.Male;

    public void OnClickMaleCharacter() => OnClickCharacterType(ECharacterType.Male);
    public void OnClickFemaleCharacter() => OnClickCharacterType(ECharacterType.Female);
    public void OnClickCharacterType(ECharacterType characterType)
    {
        SelectedCharacterType = characterType;

        MaleCharacter.SetActive(characterType == ECharacterType.Male);
        FemaleCharacter.SetActive(characterType == ECharacterType.Female);
    }


    public void Start()
    {
        OnClickFemaleCharacter();
    }

    // 방 만들기 함수를 호출
    public void OnclickMakeRoomButton()
    {
        MakeRoom();
    }

    private void MakeRoom()
    {
        string nickname = NicknameInputField.text;
        string roomName = RoomNameInputField.text;

        if(string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomName))
        {
            return;
        }

        // 포톤에 닉네임 등록
        PhotonNetwork.NickName = nickname;

        // 룸 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;    // 룸에 입장할 수 있는 최대 접속자 수
        roomOptions.IsOpen     = true;  // 룸의 오픈 여부
        roomOptions.IsVisible  = true;  // 로비에서 룸 목록에 노출시킬지 여부

        // 룸 생성
        // 룸을 생성한 유저는 자동으로 해당 룸에 입장하고 OnJoinedRoom 콜백 함수가 호출
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
}

