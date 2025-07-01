using Unity.Cinemachine;
using UnityEngine;

public class CameraShaking_Impulse : MonoBehaviour
{  
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float force = 1.0f)
    {
        _impulseSource.GenerateImpulse(force);
    }
}
