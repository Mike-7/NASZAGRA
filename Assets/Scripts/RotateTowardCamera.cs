using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void FixedUpdate()
    {
        if(target == null)
        {
            return;
        }

        transform.position = target.position + offset;
        transform.rotation = Camera.main.transform.rotation;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
