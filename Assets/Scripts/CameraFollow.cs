using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float distance = 10f;
    public float minDistance = 5f;
    public float maxDistance = 15f;

    public float lerpSpeed = 5f;
    public float scrollSpeed = 50f;

    Vector3 offset;

    void Start()
    {
        offset = new Vector3(-distance / 2, distance, distance / 2);
    }

    void Update()
    {
        distance += -Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;

        if(distance > maxDistance)
        {
            distance = maxDistance;
        }
        else if(distance < minDistance)
        {
            distance = minDistance;
        }

        offset = new Vector3(-distance / 2, distance, distance / 2);
    }

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
