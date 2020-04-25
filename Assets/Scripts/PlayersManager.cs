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
    public GameObject playerPrefab2;
    public PlayerInfo playerInfo;

    new PhotonView photonView;

    void Start()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        photonView = GetComponent<PhotonView>();

        Spawn();
    }

    void Spawn()
    {
        if(!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(NaszaGra.TEAM_ID))
        {
            PhotonNetwork.LocalPlayer.CustomProperties[NaszaGra.TEAM_ID] = 0;
        }
        if(!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(NaszaGra.CHARACTER_ID))
        {
            PhotonNetwork.LocalPlayer.CustomProperties[NaszaGra.CHARACTER_ID] = 0;
        }

        GameObject playerGameObject;

        if((int)PhotonNetwork.LocalPlayer.CustomProperties[NaszaGra.CHARACTER_ID] == 0)
        {
            playerGameObject = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.Euler(0, 0, 0));
        }
        else
        {
            playerGameObject = PhotonNetwork.Instantiate(playerPrefab2.name, Vector3.zero, Quaternion.Euler(0, 0, 0));
        }

        Player player = playerGameObject.GetComponent<Player>();
        PlayerControls playerControls = playerGameObject.GetComponent<PlayerControls>();
        PhotonView photonView = PhotonView.Get(player);

        cameraFollow.SetPlayer(playerGameObject.transform);
        playerInfo.SetPlayer(playerGameObject.GetComponent<PlayerControls>());
        playerControls.enabled = true;
        photonView.RPC("SetNickName", RpcTarget.All, PhotonNetwork.NickName,
            (int)PhotonNetwork.LocalPlayer.CustomProperties[NaszaGra.TEAM_ID]);
    }

    public IEnumerator Respawn(int teamID)
    {
        photonView.RPC("AddPointsToTeam", RpcTarget.All, teamID, -10);

        if (scoreManager.CanRespawn(teamID))
        {
            yield return new WaitForSeconds(5);
            Spawn();
        }
    }
}
