using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float lerpSpeed = 5f;

    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector3 desiredPosition = player.position + offset;

        transform.position =  Vector3.Lerp(transform.position, desiredPosition, lerpSpeed * Time.deltaTime);
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }
}
