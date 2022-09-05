using UnityEngine;

public class SwapPlayersBehaviour : PowerUp
{
    void Start()
    {
        user.GetComponent<PlayerInputBridge>().StopAllCoroutines();
        affected.GetComponent<PlayerInputBridge>().StopAllCoroutines();
        Vector3 positionAux = user.transform.position;
        user.transform.position = affected.transform.position;
        affected.transform.position = positionAux;
        Destroy(this);
    }

}
