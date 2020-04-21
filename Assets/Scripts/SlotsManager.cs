using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SlotsManager : MonoBehaviourPunCallbacks
{
    public Menu menu;
    public GameObject slotsContainer;
    public GameObject slotPrefab;

    List<GameObject> slots;

    void Start()
    {
        slots = new List<GameObject>();
    }

    public override void OnJoinedRoom()
    {
        var playerList = PhotonNetwork.PlayerList;
        for (int i = 0; i < playerList.Length; i++)
        {
            CreateSlot(playerList[i].ActorNumber);

            Slot slot = GetSlot(playerList[i].ActorNumber);
            slot.SetNickName(playerList[i].NickName);
            if (playerList[i].CustomProperties.ContainsKey(NaszaGra.CHARACTER_ID))
            {
                slot.Select((int)playerList[i].CustomProperties[NaszaGra.CHARACTER_ID]);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        CreateSlot(newPlayer.ActorNumber);

        Slot slot = GetSlot(newPlayer.ActorNumber);
        slot.SetNickName(newPlayer.NickName);
        if (newPlayer.CustomProperties.ContainsKey(NaszaGra.CHARACTER_ID))
        {
            slot.Select((int)newPlayer.CustomProperties[NaszaGra.CHARACTER_ID]);
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RemoveSlot(otherPlayer.ActorNumber);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        menu.RoomMenuBack();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        // Ignore changes for local player
        if(targetPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        object characterID;
        if (!changedProps.TryGetValue(NaszaGra.CHARACTER_ID, out characterID) || characterID == null)
        {
            return;
        }

        Slot slot = GetSlot(targetPlayer.ActorNumber);
        slot.Select((int)characterID);
    }

    public override void OnLeftRoom()
    {
        foreach(var slot in slots)
        {
            Destroy(slot);
        }

        slots.Clear();
    }

    void CreateSlot(int actorNumber)
    {
        float x = slots.Count * 3.5f - 2 * 3;
        Vector3 position = new Vector3(x, 0, 0);

        var slot = Instantiate(slotPrefab);
        slot.transform.position = position;
        slot.transform.SetParent(slotsContainer.transform);
        slot.GetComponent<Slot>().actorNumber = actorNumber;

        slots.Add(slot);
    }

    Slot GetSlot(int actorNumber)
    {
        foreach(var slotGameObject in slots)
        {
            Slot slot = slotGameObject.GetComponent<Slot>();
            if(slot.actorNumber == actorNumber)
            {
                return slot;
            }
        }

        return null;
    }

    void RemoveSlot(int actorNumber)
    {
        foreach(var slotGameObject in slots)
        {
            Slot slot = slotGameObject.GetComponent<Slot>();
            if(slot.actorNumber == actorNumber)
            {
                Destroy(slotGameObject);
                slots.Remove(slotGameObject);

                break;
            }
        }

        ResetSlotsPositions();
    }

    void ResetSlotsPositions()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            float x = i * 3 - 2 * 3;

            slots[i].transform.position = new Vector3(x, 0, 0);
        }
    }

    public void Select(int characterID)
    {
        Hashtable props = new Hashtable
        {
            { NaszaGra.CHARACTER_ID, characterID }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        Slot slot = GetSlot(PhotonNetwork.LocalPlayer.ActorNumber);
        slot.Select((int)characterID);
    }
}
