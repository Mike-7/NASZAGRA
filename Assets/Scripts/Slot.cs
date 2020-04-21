using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Slot : MonoBehaviourPunCallbacks
{
    public int actorNumber;
    public GameObject[] charactersPrefabs;
    public GameObject nickNameTextPrefab;

    GameObject selectedCharacter;
    GameObject nickNameText;

    public void SetNickName(string nickName)
    {
        if(nickNameText == null)
        {
            nickNameText = Instantiate(nickNameTextPrefab);
            nickNameText.transform.SetParent(transform, false);
        }

        nickNameText.GetComponent<TextMeshPro>().text = nickName;
    }

    public void Select(int characterID)
    {
        if (selectedCharacter != null)
        {
            Destroy(selectedCharacter);
        }

        switch(characterID)
        {
            case 0:
                selectedCharacter = Instantiate(charactersPrefabs[0], transform, false);
                break;

            case 1:
                selectedCharacter = Instantiate(charactersPrefabs[1], transform, false);
                break;
        }
    }
}
