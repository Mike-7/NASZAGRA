using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float timer = 5f;
    float timeStamp;

    bool isNetworkObject = true;
    PhotonView photonView;

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

        if(isNetworkObject)
        {
            photonView = PhotonView.Get(this);
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

        if(photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        if(isNetworkObject && Time.time - timeStamp >= timer)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
