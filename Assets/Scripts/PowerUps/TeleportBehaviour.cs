using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBehaviour : PowerUp
{
    void Start()
    {
        user.transform.position += new Vector3(0,0,Random.Range(4, 10));
        Destroy(this);
    }
}
