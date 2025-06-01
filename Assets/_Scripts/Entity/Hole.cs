using System;
using UnityEngine;

public class Hole: MonoBehaviour
{
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * -1.5f);
    }
}