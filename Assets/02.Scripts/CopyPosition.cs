using Photon.Pun;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    [SerializeField]
    private bool _x,_y,_z;  // 이 값이 true이면 target의 좌표, false이면 현재 좌표를 그대로 사용
    [SerializeField]
    private Transform _target;  // 쫓아가야할 대상 Transform

    private void Update()
    {
        if (!_target) return;

        transform.position = new Vector3(
            (_x ? _target.position.x : transform.position.x),
            (_y ? _target.position.y : transform.position.y),
            (_z ? _target.position.z : transform.position.z)
        );
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
