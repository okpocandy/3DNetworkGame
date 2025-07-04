using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerAttackAbility _attackAbility;

    public GameObject HitEffect;
    public float SizeMultiplier = 0.5f;

    private void Start()
    {
        _attackAbility = GetComponentInParent<PlayerAttackAbility>();
        ScoreManager.Instance.OnDataChanged += WeaponSizeChange;
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 구독 해제
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnDataChanged -= WeaponSizeChange;
        }
    }

    private void WeaponSizeChange()
    {
        // 오브젝트가 파괴되었거나 transform이 null인 경우 처리하지 않음
        if (this == null || transform == null)
        {
            return;
        }
        
        float sizeFactor = (float)ScoreManager.Instance.TotalScore / 10000f;
        transform.localScale = new Vector3(1f, 1f, 1f) + new Vector3(sizeFactor * SizeMultiplier, sizeFactor * SizeMultiplier, sizeFactor * SizeMultiplier);
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
            
            // 트리거 접촉 위치 추정
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Instantiate(HitEffect, hitPoint, Quaternion.identity);
        }
    }
}
