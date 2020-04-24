using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(DamageAnimation))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    public GameObject nickNamePrefab;
    public GameObject smokePrefab;
    public GameObject speakerPrefab;
    public GameObject toilerPrefab;

    public bool isPlayable = false;

    public float speed = 8f;
    public float stunSpeed = 0.5f;
    public float rotationSpeed = 5f;
    public int health = 100;

    public float attack1Cooldown = 3;
    public float attack1TimeStamp = 0;

    public float attack2Cooldown = 5;
    public float attack2TimeStamp = 0;

    public float attack3Cooldown = 30;
    public float attack3TimeStamp = 0;

    public float smokeDamageCooldown = 0.5f;
    public float smokeDamageTimestamp = 0;

    public ParticleSystem smokeParticle;

    CharacterController characterController;
    Animator animator;
    DamageAnimation damageAnimation;
    AudioSource audioSource;
    GameObject nickNameText;

    Vector3 moveDir = Vector3.zero;
    Quaternion rotation;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        damageAnimation = GetComponent<DamageAnimation>();
        audioSource = GetComponent<AudioSource>();

        rotation = Quaternion.Euler(0, 0, 0);
        attack1TimeStamp = Time.time;
        attack2TimeStamp = Time.time;
        attack3TimeStamp = Time.time;

        nickNameText = Instantiate(nickNamePrefab);
        nickNameText.GetComponent<RotateTowardCamera>().SetTarget(transform);
    }

    void Update()
    {
        if(!isPlayable)
        {
            return;
        }

        rotation = GetRotationTowardsMouse();
        animator.SetBool("walking", moveDir != Vector3.zero);
    }

    void FixedUpdate()
    {
        if(!isPlayable)
        {
            return;
        }

        bool stun = false;
        var effects = PlayersEffects._instance.GetActiveEffects(PhotonNetwork.LocalPlayer.ActorNumber);
        foreach (var effect in effects)
        {
            if (effect.effectName == "stun")
            {
                stun = true;
            }
        }

        if(stun)
        {
            characterController.Move(moveDir * stunSpeed * Time.deltaTime);
        }
        else
        {
            characterController.Move(moveDir * speed * Time.deltaTime);
        }
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation,
            rotation, rotationSpeed * Time.deltaTime);
    }

    void OnDestroy()
    {
        Destroy(nickNameText);
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    Quaternion GetRotationTowardsMouse()
    {
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 mouseOnScreen = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
        Quaternion rotation = Quaternion.Euler(0, -angle + 45, 0);

        return rotation;
    }

    Vector3 TranslateMoveVector2D(Vector2 move2D)
    {
        return new Vector3(move2D.y - move2D.x, 0, -move2D.y - move2D.x);
    }

    public void Move(InputAction.CallbackContext context)
    {
        var move2D = context.ReadValue<Vector2>();

        moveDir = TranslateMoveVector2D(move2D);
    }

    [PunRPC]
    void EmitSmoke()
    {
        smokeParticle.Play();
    }

    public void Attack1(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            return;
        }

        if(Time.time - attack1TimeStamp >= attack1Cooldown)
        {
            attack1TimeStamp = Time.time;

#if UNITY_EDITOR
            if(!PhotonNetwork.InRoom)
            {
                EmitSmoke();
                return;
            }
#endif
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EmitSmoke", RpcTarget.All);
        }
    }

    public void Attack2(InputAction.CallbackContext context)
    {
        if (Time.time - attack2TimeStamp >= attack2Cooldown)
        {
            attack2TimeStamp = Time.time;

#if UNITY_EDITOR
            if (!PhotonNetwork.InRoom)
            {
                PlayersEffects._instance.AddEffect("stun", PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                PhotonView photonView = PhotonView.Get(PlayersEffects._instance);
                photonView.RPC("AddEffect", RpcTarget.All, "stun", PhotonNetwork.LocalPlayer.ActorNumber);
            }
#else
            PhotonView photonView = PhotonView.Get(PlayersEffects._instance);
            photonView.RPC("AddEffect", RpcTarget.All, "stun", PhotonNetwork.LocalPlayer.ActorNumber);
#endif

            Vector3 offset = new Vector3(1, 5, 1);
#if UNITY_EDITOR
            if (!PhotonNetwork.InRoom)
            {
                var speaker = Instantiate(speakerPrefab, transform.position + offset, toilerPrefab.transform.rotation);
                return;
            }
#endif
            PhotonNetwork.Instantiate(speakerPrefab.name, transform.position + offset, toilerPrefab.transform.rotation);
        }
    }

    public void Attack3(InputAction.CallbackContext context)
    {
        if (Time.time - attack3TimeStamp >= attack3Cooldown)
        {
            attack3TimeStamp = Time.time;

#if UNITY_EDITOR
            if (!PhotonNetwork.InRoom)
            {
                Instantiate(toilerPrefab, transform.position + new Vector3(0, 10, 0), toilerPrefab.transform.rotation);
                return;
            }
#endif
            PhotonNetwork.Instantiate(toilerPrefab.name, transform.position + new Vector3(0, 10, 0), toilerPrefab.transform.rotation);
        }
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

    [PunRPC]
    public void SetNickName(string nickName)
    {
        nickNameText.GetComponent<TextMeshPro>().text = nickName;
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

    public void TakeDamage(int damageValue)
    {
        Camera.main.GetComponent<CameraFollow>().Shake();
#if UNITY_EDITOR
        if (!PhotonNetwork.InRoom)
        {
            PlayDamageSound();
        }
        else
        {
            PhotonView photonView2 = PhotonView.Get(this);
            photonView2.RPC("PlayDamageSound", RpcTarget.All);
        }
#else
        PhotonView photonView2 = PhotonView.Get(this);
        photonView2.RPC("PlayDamageSound", RpcTarget.All);
#endif

        health -= damageValue;
        if (health <= 0)
        {
            Die();
        }

#if UNITY_EDITOR
        if (!PhotonNetwork.InRoom)
        {
            PlayDamageAnimation();
            return;
        }
#endif

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("PlayDamageAnimation", RpcTarget.All);
    }

    void OnParticleCollision(GameObject other)
    {
        if(!isPlayable)
        {
            return;
        }

        // Check if player owns particle system
        if (other.transform.parent == transform)
        {
            return;
        }

        if (Time.time - smokeDamageTimestamp >= smokeDamageCooldown)
        {
            smokeDamageTimestamp = Time.time;
            TakeDamage(10);
        }
    }
}
