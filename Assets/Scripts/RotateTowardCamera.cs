using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public float lerpSpeed = 5f;

    void Start()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
    void FixedUpdate()
    {
        if(target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, lerpSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
