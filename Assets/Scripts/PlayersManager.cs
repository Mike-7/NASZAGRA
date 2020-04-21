using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayersManager : MonoBehaviourPunCallbacks
{
    public CameraFollow cameraFollow;
    public GameObject playerPrefab;
    public PlayerInfo playerInfo;

    void Start()
    {
#if UNITY_EDITOR
        if(!PhotonNetwork.InRoom)
        {
            GameObject offlinePlayerGameObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
            cameraFollow.SetPlayer(offlinePlayerGameObject.transform);
            Player offlinePlayer = offlinePlayerGameObject.GetComponent<Player>();
            offlinePlayer.enabled = true;
            playerInfo.SetPlayer(offlinePlayer);

            return;
        }
#endif

        var playerGameObject = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.Euler(0, 0, 0));
        cameraFollow.SetPlayer(playerGameObject.transform);
        Player player = playerGameObject.GetComponent<Player>();
        player.enabled = true;
        playerInfo.SetPlayer(player);
    }
}
