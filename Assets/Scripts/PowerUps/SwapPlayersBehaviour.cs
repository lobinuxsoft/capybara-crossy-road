using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapPlayersBehaviour : PowerUp
{
    void Start()
    {
        Vector3 positionAux = user.transform.position;
        user.transform.position = affected.transform.position;
        affected.transform.position = positionAux;
        Destroy(this);
    }

}
