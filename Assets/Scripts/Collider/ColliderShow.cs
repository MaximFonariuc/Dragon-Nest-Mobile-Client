#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ColliderShow : MonoBehaviour {

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, 1);

        Collider[] allCollider = gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider collider in allCollider)
        {
            BoxCollider b = collider as BoxCollider;

            if (b != null)
            {
                Transform go = b.transform;

                Matrix4x4 rotationMatrix = go.localToWorldMatrix;
                Gizmos.matrix = rotationMatrix;

                //Gizmos.DrawWireCube(go.position, new Vector3(go.localScale.x * b.size.x, go.localScale.y * b.size.y, go.localScale.z * b.size.z));
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }

            Gizmos.matrix = Matrix4x4.identity;

            CapsuleCollider c = collider as CapsuleCollider;
            if (c != null)
            {
                Transform go = c.transform;

                //Matrix4x4 rotationMatrix = go.localToWorldMatrix;
                //Gizmos.matrix = rotationMatrix;

                Gizmos.DrawWireSphere(go.position + new Vector3(0, 50, 0), go.localScale.x * c.radius);


            }
        }
    }
}
#endif