using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    public static CameraShaking Instance;
    public CinemachineCamera MyCamera;
    private CinemachineBasicMultiChannelPerlin _noise;


    private void Awake()
    {
        Instance = this;

        MyCamera = GetComponent<CinemachineCamera>();
        _noise = MyCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float intensity, float frequency, float duration)
    {
        _noise.AmplitudeGain = intensity;
        _noise.FrequencyGain = frequency;

        StartCoroutine(Shake_Coroutine(intensity, frequency, duration));
    }

    private IEnumerator Shake_Coroutine(float intensity, float frequency, float duration)
    {
        _noise.AmplitudeGain = intensity;
        _noise.FrequencyGain = frequency;
        yield return new WaitForSeconds(duration);
        _noise.AmplitudeGain = 0f;
        _noise.FrequencyGain = 0f;
    }
}
