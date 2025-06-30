using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<Player>(out Player player))
        {
            if(player.State == EPlayerState.Death)
            {
                return;
            }
            Debug.Log("DeadZone");

            player.Damaged(999999999);
        }
    }
}
