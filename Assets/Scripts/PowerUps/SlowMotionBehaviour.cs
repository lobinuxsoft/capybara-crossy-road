using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionBehaviour : PowerUp
{
    private void Start()
    {
        Time.timeScale = 0.5f;
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
