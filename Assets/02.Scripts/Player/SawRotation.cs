using UnityEngine;

public class SawRotation : MonoBehaviour
{
    public float RotationSpeed = 360f;
    private void Update()
    {
        transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
    }
}
