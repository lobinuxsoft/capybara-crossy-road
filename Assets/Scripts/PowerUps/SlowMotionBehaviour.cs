using System.Collections;
using UnityEngine;

public class SlowMotionBehaviour : PowerUp
{
    private void Start()
    {
        Time.timeScale = 0.1f;
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        float time = 0;
        while(time < 5)
        {
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1;
        Destroy(this);
    }
}
