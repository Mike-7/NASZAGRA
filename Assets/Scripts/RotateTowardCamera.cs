using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Start()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
    void LateUpdate()
    {
        if(target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = desiredPosition;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
