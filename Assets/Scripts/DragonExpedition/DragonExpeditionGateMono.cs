using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class DragonExpeditionGateMono : MonoBehaviour
{
    Rigidbody rb;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
	}
}
