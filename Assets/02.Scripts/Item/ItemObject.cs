using UnityEngine;
using Photon.Pun;


public enum EItemType
{
    Score,
    Health,
    Stamina
}

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class ItemObject : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if(player.State == EPlayerState.Death)
            {
                return;
            }

            // 하위 클래스에서 구현할 메서드들 호출
            ApplyItemEffect(player);
            
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }

    // 하위 클래스에서 반드시 구현할 메서드들
    protected abstract void ApplyItemEffect(Player player);
}