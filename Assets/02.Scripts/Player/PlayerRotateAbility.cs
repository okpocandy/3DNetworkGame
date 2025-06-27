using Unity.Cinemachine;
using UnityEngine;

public class PlayerRotateAbility : PlayerAbility
{
    // 마우스를 조작하면 캐릭터/카메라를 그 방향으로 회전시키고 싶다.
    public Transform CameraRoot;
    // 마우스 입력값을 누적할 변수수
    private float _mx;
    private float _my;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if(_photonView.IsMine)
        {
            var camera = GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineCamera>();
            camera.Follow = CameraRoot;
        }
    }

    private void Update()
    {
        if(!_photonView.IsMine)
        {
            return;
        }
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _mx += mouseX * _owner.Stat.RoationSpeed * Time.deltaTime;
        _my += mouseY * _owner.Stat.RoationSpeed * Time.deltaTime;

        _my = Mathf.Clamp(_my, -85f, 85f);

        // y축 회전은 캐릭터만 한다. ( 캐릭터가 회전하면 카메라도 따라서 회전한다. )
        transform.eulerAngles = new Vector3(0, _mx, 0);

        // x축 회전은 카메라만 한다. (로컬 회전 사용)
        CameraRoot.localEulerAngles = new Vector3(-_my, 0, 0);
    }
}
