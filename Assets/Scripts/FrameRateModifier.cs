using UnityEngine;

public class FrameRateModifier : MonoBehaviour
{
    [Tooltip("-1 la plataforma destino usa su default frame rate")]
    [SerializeField, Min(-1)] int frameRate = 60;

    private void Awake() 
    { 
        SetFrameRate(frameRate);
    }

    public void SetFrameRate(int targetFrameRate)
    {
        targetFrameRate = targetFrameRate > Screen.currentResolution.refreshRate ? Screen.currentResolution.refreshRate : targetFrameRate;

        Application.targetFrameRate = targetFrameRate;

        Debug.Log($"Frame Rate: {Application.targetFrameRate} Hz");
    }
}