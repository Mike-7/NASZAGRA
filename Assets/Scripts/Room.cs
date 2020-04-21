using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Room : MonoBehaviour
{
    public string roomName;

    public void Join()
    {
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Error while joining to the room");
            return;
        }
        
        PhotonNetwork.JoinRoom(roomName);
    }
}
