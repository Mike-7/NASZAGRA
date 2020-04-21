using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomsManager : MonoBehaviourPunCallbacks
{
    public RectTransform roomsContainer;
    public GameObject roomPrefab;

    List<GameObject> roomsCache;

    void Start()
    {
        roomsCache = new List<GameObject>();
    }

    bool RoomExists(string roomName)
    {
        foreach(var room in roomsCache)
        {
            if(room.GetComponent<Room>().roomName == roomName)
            {
                return true;
            }
        }

        return false;
    }

    void CreateRoom(string roomName)
    {
        if(RoomExists(roomName))
        {
            return;
        }

        var newRoom = Instantiate(roomPrefab);

#if UNITY_EDITOR
        newRoom.GetComponentInChildren<TextMeshProUGUI>().text = roomName;
#else
        newRoom.GetComponentInChildren<TextMeshProUGUI>().text = roomName.Remove(roomName.Length - 6);
#endif
        newRoom.GetComponent<Room>().roomName = roomName;
        newRoom.GetComponent<RectTransform>().SetParent(roomsContainer, false);

        roomsCache.Add(newRoom);
    }

    void RemoveRoom(string roomName)
    {
        foreach(var room in roomsCache)
        {
            if(room.GetComponent<Room>().roomName == roomName)
            {
                Destroy(room);
                roomsCache.Remove(room);
                break;
            }
        }
    }

    void ResetRoomsPositions()
    {
        float y = -30;

        foreach(var room in roomsCache)
        {
            room.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, y, 0);

            y -= 70;
        }

        float h = roomsCache.Count * 70;
        if(h < 420)
        {
            h = 420;
        }

        roomsContainer.sizeDelta =
            new Vector2(0, h);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if(room.RemovedFromList)
            {
                RemoveRoom(room.Name);
                continue;
            }

            CreateRoom(room.Name);
        }

        ResetRoomsPositions();
    }
}
