using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Menu : MonoBehaviourPunCallbacks
{
    public GameObject mainMenu;
    public GameObject roomMenu;
    public GameObject roomListMenu;
    public GameObject settingsMenu;

    public Button[] buttonsRequiringConnection;
    public GameObject playButton;

    public override void OnConnectedToMaster()
    {
        foreach(var button in buttonsRequiringConnection)
        {
            button.interactable = true;
        }
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }

        mainMenu.SetActive(false);
        roomListMenu.SetActive(false);
        settingsMenu.SetActive(false);

        roomMenu.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        roomMenu.SetActive(false);
        roomListMenu.SetActive(false);
        settingsMenu.SetActive(false);

        mainMenu.SetActive(true);
    }

    public void RoomMenu()
    {
        if(!PhotonNetwork.IsConnected || PhotonNetwork.InRoom
            || PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.ConnectingToGameServer
            || PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joining
            || PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Authenticating)
        {
            return;
        }

        if (!PhotonNetwork.CreateRoom(PhotonNetwork.NickName + Random.Range(100000, 999999 + 1)))
        {
            Debug.LogError("Room creation failed");
            return;
        }
    }

    public void RoomListMenu()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        if(!roomListMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
        }

        roomListMenu.SetActive(!roomListMenu.activeSelf);
    }

    public void SettingsMenu()
    {
        if(!settingsMenu.activeSelf)
        {
            roomListMenu.SetActive(false);
        }

        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    public void RoomMenuPlay()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void RoomMenuBack()
    {
        roomMenu.SetActive(false);
        mainMenu.SetActive(true);

        Hashtable props = new Hashtable
        {
            { NaszaGra.CHARACTER_ID, null }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        PhotonNetwork.LeaveRoom();
    }

    public void MainMenuExit()
    {
        Application.Quit();
    }
}
