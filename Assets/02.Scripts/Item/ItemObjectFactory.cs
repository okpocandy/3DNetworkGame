using Photon.Pun;
using UnityEngine;

// 아이템 공장: 아이템 생성을 담당
[RequireComponent(typeof(PhotonView))]
public class ItemObjectFactory : MonoBehaviourPun
{
    private static ItemObjectFactory _instance;
    public static ItemObjectFactory Instance => _instance;

    private PhotonView _photonView;
    private float _timer = 0f;
    public float ItemCreateInterval = 5f;
    public Vector3 ItemCreatePosition;

    private void Awake()
    {
        _instance = this;
        
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        _timer += Time.deltaTime;
        if (_timer >= ItemCreateInterval)
        {
            _timer = 0f;
            RequestCreate((EItemType)Random.Range(0, (int)EItemType.Count),
            new Vector3(Random.Range(-ItemCreatePosition.x, ItemCreatePosition.x),
            ItemCreatePosition.y,
            Random.Range(-ItemCreatePosition.z, ItemCreatePosition.z)));
        }
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
        PhotonView photonView = PhotonView.Find(viewId);
        if (photonView == null)
        {
            Debug.LogWarning($"PhotonView with ID {viewId} not found. Item may have already been destroyed.");
            return;
        }
        
        GameObject objectToDelete = photonView.gameObject;
        if (objectToDelete == null)
        {
            Debug.LogWarning($"GameObject for PhotonView {viewId} is null.");
            return;
        }
        
        PhotonNetwork.Destroy(objectToDelete);
    }
}