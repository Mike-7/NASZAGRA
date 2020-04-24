using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float timer = 5f;
    float timeStamp;

    bool isNetworkObject = true;

    void Start()
    {
        timeStamp = Time.time;

#if UNITY_EDITOR
        if (!PhotonNetwork.InRoom)
        {
            Destroy(gameObject, timer);
            return;
        }
#endif

        if (GetComponent<PhotonView>() == null)
        {
            isNetworkObject = false;
            Destroy(gameObject, timer);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if(!PhotonNetwork.InRoom)
        {
            return;
        }
#endif

        if(isNetworkObject && Time.time - timeStamp >= timer)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
