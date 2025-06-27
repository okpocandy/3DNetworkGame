using Photon.Pun;
using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    protected Player _owner { get; private set; }
    protected PhotonView _photonView { get; private set; }

    
    protected virtual void Awake()
    {
        _owner = GetComponent<Player>();
        _photonView = GetComponent<PhotonView>();
    }

}
