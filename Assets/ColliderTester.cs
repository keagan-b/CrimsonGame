using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ColliderTester : NetworkBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("COLLIDING");
    }
}
