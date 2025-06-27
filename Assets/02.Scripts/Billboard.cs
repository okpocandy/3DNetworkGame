using UnityEngine;

public class Billboard : MonoBehaviour
{
    // 캔버스에 달아야 한다.
    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
