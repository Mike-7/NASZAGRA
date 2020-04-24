using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineMode : MonoBehaviourPunCallbacks
{
#if UNITY_EDITOR
    void Start()
    {
        if(PhotonNetwork.InRoom)
        {
            return;
        }

        PhotonNetwork.OfflineMode = true;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.CreateRoom("OfflineRoom");
    }
#endif
}
