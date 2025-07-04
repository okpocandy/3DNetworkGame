using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    public List<UI_ScoreSlot> Slots;
    public UI_ScoreSlot MySlot;

    private void Start()
    {
        ScoreManager.Instance.OnDataChanged += Refresh;
    }

    private void Refresh()
    {
        Dictionary<string, int> scores = ScoreManager.Instance.Scores;
        var sortedScores = scores.OrderByDescending(x => x.Value).ToList();

        // 1~4위 점수 등록록
        for(int i=0; i<Slots.Count; i++)
        {
            if(i < sortedScores.Count)
            {
                Slots[i].gameObject.SetActive(true);
                Slots[i].Set($"{i+1}", sortedScores[i].Key, sortedScores[i].Value);
            }
            else
            {
                Slots[i].gameObject.SetActive(false);
            }
        }

        // 내 점수 등록
        string myNickName = PhotonNetwork.NickName + "_" + PhotonNetwork.LocalPlayer.ActorNumber;
        int index = sortedScores.FindIndex(x => x.Key == myNickName);
        if(index < 0) return;
        MySlot.Set($"{index + 1}", sortedScores[index].Key, sortedScores[index].Value);
    }
}
