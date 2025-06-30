using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerAttackAbility _attackAbility;

    private void Start()
    {
        _attackAbility = GetComponentInParent<PlayerAttackAbility>();
    }


    private void OnTriggerEnter(Collider other)
    {
        // 자기 자신과 부딛혔다면 무시
        if(other.transform == _attackAbility.transform)
        {
            return;
        }

        IDamaged damagedObject = other.GetComponent<IDamaged>();
        if(damagedObject != null)
        {
            _attackAbility.Hit(other);
        }
    }
}
