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

    List<GameObject> players;

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
            offlinePlayerGameObject.GetComponent<PlayerControls>().enabled = true;
            offlinePlayer.SetNickName(NaszaGra.DEFAULT_NICK_NAME, (int)PhotonNetwork.LocalPlayer.CustomProperties[NaszaGra.TEAM_ID]);
            playerInfo.SetPlayer(offlinePlayerGameObject.GetComponent<PlayerControls>());

            return;
        }
#endif

        var playerGameObject = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.Euler(0, 0, 0));
        cameraFollow.SetPlayer(playerGameObject.transform);
        Player player = playerGameObject.GetComponent<Player>();
        playerGameObject.GetComponent<PlayerControls>().enabled = true;
        PhotonView photonView = PhotonView.Get(player);
        photonView.RPC("SetNickName", RpcTarget.All, PhotonNetwork.NickName, (int)PhotonNetwork.LocalPlayer.CustomProperties[NaszaGra.TEAM_ID]);
        playerInfo.SetPlayer(playerGameObject.GetComponent<PlayerControls>());
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
