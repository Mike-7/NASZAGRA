using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(DamageAnimation))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    public GameObject nickNamePrefab;
    public GameObject smokePrefab;
    public GameObject speakerPrefab;
    public GameObject toilerPrefab;
    public ParticleSystem smokeParticle;

    public int health = 100;
    public int teamID = 0;
    public float smokeDamageCooldown = 0.5f;
    public float smokeDamageTimestamp = 0;

    PhotonView photonView;
    DamageAnimation damageAnimation;
    AudioSource audioSource;
    GameObject nickNameText;

    void Awake()
    {
        photonView = PhotonView.Get(this);

        damageAnimation = GetComponent<DamageAnimation>();
        audioSource = GetComponent<AudioSource>();

        nickNameText = Instantiate(nickNamePrefab);
        nickNameText.GetComponent<RotateTowardCamera>().SetTarget(transform);
    }

    void OnDestroy()
    {
        Destroy(nickNameText);
    }

    public void SpawnToilet()
    {
        PhotonNetwork.Instantiate(toilerPrefab.name, transform.position + new Vector3(0, 5, 0), toilerPrefab.transform.rotation);
    }

    public void SpawnSpeaker()
    {
        PhotonNetwork.Instantiate(speakerPrefab.name, transform.position + new Vector3(1, 1, 1), speakerPrefab.transform.rotation);
    }

    [PunRPC]
    public void EmitSmoke()
    {
        smokeParticle.Play();
    }

    [PunRPC]
    public void SetNickName(string nickName, int teamID)
    {
        this.teamID = teamID;

        nickNameText.GetComponent<TextMeshPro>().text = "";
        if (teamID == 0)
        {
            nickNameText.GetComponent<TextMeshPro>().text += "<color=\"orange\">";
        }

        nickNameText.GetComponent<TextMeshPro>().text += nickName;
    }

    [PunRPC]
    public void PlayDamageSound()
    {
        audioSource.Play();
    }

    [PunRPC]
    public void PlayDamageAnimation()
    {
        damageAnimation.OnDamage();
    }

    void Die()
    {
        if(!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(NaszaGra.TEAM_ID))
        {
            PhotonNetwork.LocalPlayer.CustomProperties[NaszaGra.TEAM_ID] = 0;
        }

        PlayersManager._instance.StartCoroutine(
            PlayersManager._instance.Respawn(
                (int)PhotonNetwork.LocalPlayer.CustomProperties[NaszaGra.TEAM_ID]));

        PhotonNetwork.Destroy(gameObject);
    }

    public void TakeDamage(int damageValue)
    {
        Camera.main.GetComponent<CameraFollow>().Shake();
        photonView.RPC("PlayDamageSound", RpcTarget.All);
        photonView.RPC("PlayDamageAnimation", RpcTarget.All);

        health -= damageValue;
        if (health <= 0)
        {
            Die();
        }
    }

    void OnParticleCollision(GameObject other)
    {
        // Damage is always handled by local player
        if(photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        // Check if player owns particle system
        if (other.transform.parent == transform)
        {
            return;
        }

        var player = other.transform.parent.GetComponent<Player>();
        if (player != null)
        {
            if(player.teamID == teamID)
            {
                // Same team (No damage)
                return;
            }
        }

        if (Time.time - smokeDamageTimestamp >= smokeDamageCooldown)
        {
            smokeDamageTimestamp = Time.time;
            TakeDamage(10);
        }
    }
}
