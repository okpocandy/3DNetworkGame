using System.Collections;
using UnityEngine;

public class PlayerShakeAbility : PlayerAbility
{
    // 무엇을 어떤 힘으로 몇초동안 흔들 것인가
    public Transform Target;
    public float Strength;
    public float Duration;

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake_Coroutine());
    }

    private IEnumerator Shake_Coroutine()
    {
        float elapsedTime = 0f;

        // 초기값 저장
        Vector3 startPosition = Target.localPosition;

        while (elapsedTime <= Duration)
        {
            elapsedTime += Time.deltaTime;

            // SHAKE
            Vector3 randomPosition = Random.insideUnitSphere.normalized * Strength;
            randomPosition.y = startPosition.y;
            Target.localPosition = randomPosition;

            yield return null;
        }

        Target.localPosition = startPosition;
    }
}
