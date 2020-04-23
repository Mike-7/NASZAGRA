using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayersManager : MonoBehaviourPunCallbacks
{
    public static PlayersManager _instance;

    public ScoreManager scoreManager;
    public CameraFollow cameraFollow;
    public GameObject playerPrefab;
    public PlayerInfo playerInfo;

    void Start()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        Spawn();
    }

    void Spawn()
    {
#if UNITY_EDITOR
        if (!PhotonNetwork.InRoom)
        {
            GameObject offlinePlayerGameObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
            cameraFollow.SetPlayer(offlinePlayerGameObject.transform);
            Player offlinePlayer = offlinePlayerGameObject.GetComponent<Player>();
            offlinePlayer.isPlayable = true;
            playerInfo.SetPlayer(offlinePlayer);

            return;
        }
#endif

        var playerGameObject = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.Euler(0, 0, 0));
        cameraFollow.SetPlayer(playerGameObject.transform);
        Player player = playerGameObject.GetComponent<Player>();
        player.isPlayable = true;
        playerInfo.SetPlayer(player);
    }

    public IEnumerator Respawn(int teamID)
    {
#if UNITY_EDITOR
            if (!PhotonNetwork.InRoom)
            {
                scoreManager.AddPointsToTeam(teamID, -10);
            }
            else
            {
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("AddPointsToTeam", RpcTarget.All, teamID, -10);
            }
#else
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("AddPointsToTeam", RpcTarget.All, teamID, -10);
#endif

        if (scoreManager.CanRespawn(teamID))
        {
            yield return new WaitForSeconds(5);
            Spawn();
        }
    }
}
