using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PhotonView))]
public class PlayerControls : MonoBehaviour
{
    public float speed = 8f;
    public float stunSpeed = 0.5f;
    public float rotationSpeed = 5f;
    public float[] attacksCooldowns;

    Player player;
    CharacterController characterController;
    Animator animator;
    PhotonView photonView;
    Vector3 moveDir = Vector3.zero;
    Quaternion rotation;

    float[] attacksTimestamps;

    void Awake()
    {
        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        photonView = PhotonView.Get(this);

        rotation = Quaternion.Euler(0, 0, 0);

        attacksTimestamps = new float[3];
        for(int i = 0; i < attacksTimestamps.Length; i++)
        {
            attacksTimestamps[i] = Time.time;
        }
    }

    void Update()
    {
        rotation = GetRotationTowardsMouse();
        animator.SetBool("walking", moveDir != Vector3.zero);
    }

    void FixedUpdate()
    {
        bool stun = false;
        var effects = PlayersEffects._instance.GetActiveEffects(PhotonNetwork.LocalPlayer.ActorNumber);
        foreach (var effect in effects)
        {
            if (effect.effectName == "stun")
            {
                stun = true;
            }
        }

        if (stun)
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

    public float GetCooldownTime(int index)
    {
        float attackCooldownTime = -(Time.time - attacksTimestamps[index] - attacksCooldowns[index]);
        if (attackCooldownTime <= 0)
        {
            attackCooldownTime = 0;
        }
        attackCooldownTime = attackCooldownTime * 10f / 10f;

        return attackCooldownTime;
    }

    public int GetHealth()
    {
        return player.health;
    }

    public void Move(InputAction.CallbackContext context)
    {
        var move2D = context.ReadValue<Vector2>();

        moveDir = TranslateMoveVector2D(move2D);
    }

    public void Attack1(InputAction.CallbackContext context)
    {
        if(Time.time - attacksCooldowns[0] >= attacksTimestamps[0])
        {
            photonView.RPC("EmitSmoke", RpcTarget.All);

            attacksTimestamps[0] = Time.time;
        }
    }

    public void Attack2(InputAction.CallbackContext context)
    {
        if (Time.time - attacksCooldowns[1] >= attacksTimestamps[1])
        {
            player.SpawnSpeaker();

            PhotonView photonView = PhotonView.Get(PlayersEffects._instance);
            photonView.RPC("AddEffect", RpcTarget.All, "stun", PhotonNetwork.LocalPlayer.ActorNumber);

            attacksTimestamps[1] = Time.time;
        }
    }

    public void Attack3(InputAction.CallbackContext context)
    {
        if (Time.time - attacksCooldowns[2] >= attacksTimestamps[2])
        {
            player.SpawnToilet();

            attacksTimestamps[2] = Time.time;
        }
    }

    Vector3 TranslateMoveVector2D(Vector2 move2D)
    {
        return new Vector3(move2D.y - move2D.x, 0, -move2D.y - move2D.x);
    }

    Quaternion GetRotationTowardsMouse()
    {
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 mouseOnScreen = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
        Quaternion rotation = Quaternion.Euler(0, -angle + 45, 0);

        return rotation;
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
