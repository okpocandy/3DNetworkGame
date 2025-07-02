using Photon.Pun;
using UnityEngine;

// 아이템 공장: 아이템 생성을 담당
[RequireComponent(typeof(PhotonView))]
public class ItemObjectFactory : MonoBehaviourPun
{
    private static ItemObjectFactory _instance;
    public static ItemObjectFactory Instance => _instance;

    private PhotonView _photonView;

    private void Awake()
    {
        _instance = this;
        
        _photonView = GetComponent<PhotonView>();
    }

    public void RequestCreate(EItemType itemType, Vector3 dropPosition)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Create(itemType, dropPosition);
        }
        else
        {
            _photonView.RPC(nameof(Create), RpcTarget.MasterClient, itemType, dropPosition);
        }
    }

    [PunRPC]
    private void Create(EItemType itemType, Vector3 dropPosition)
    {
        PhotonNetwork.InstantiateRoomObject($"{itemType}Item", dropPosition, Quaternion.identity);
    }
    
    public void RequestDelete(int viewId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Delete(viewId);
        }
        else
        {
            _photonView.RPC(nameof(Delete), RpcTarget.MasterClient, viewId);
        }
    }
    
    [PunRPC]
    private void Delete(int viewId)
    {
        GameObject objectToDelete = PhotonView.Find(viewId).gameObject;
        if (objectToDelete == null) return;
        
        PhotonNetwork.Destroy(objectToDelete);
    }
}