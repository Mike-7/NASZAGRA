using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public GameObject smokePrefab;
    public GameObject speakerPrefab;
    public GameObject toilerPrefab;

    public float speed = 8f;
    public float rotationSpeed = 5f;
    public uint health = 100;

    public float attack1Cooldown = 3;
    public float attack1TimeStamp = 0;

    public float attack2Cooldown = 5;
    public float attack2TimeStamp = 0;

    public float attack3Cooldown = 30;
    public float attack3TimeStamp = 0;

    CharacterController characterController;
    Animator animator;

    Vector3 moveDir = Vector3.zero;
    Quaternion rotation;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        rotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        rotation = GetRotationTowardsMouse();

        animator.SetBool("walking", moveDir != Vector3.zero);
    }

    void FixedUpdate()
    {
        characterController.Move(moveDir * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation,
            rotation, rotationSpeed * Time.deltaTime);
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

    public void Attack1(InputAction.CallbackContext context)
    {
        if(Time.time - attack1TimeStamp >= attack1Cooldown)
        {
            attack1TimeStamp = Time.time;
            Vector3 rotation = new Vector3(smokePrefab.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            Vector3 offset = new Vector3(0, 2, 0);

#if UNITY_EDITOR
            if(!PhotonNetwork.InRoom)
            {
                Instantiate(smokePrefab, transform.position + offset, Quaternion.Euler(rotation));
                return;
            }
#endif
            PhotonNetwork.Instantiate(smokePrefab.name, transform.position + offset, Quaternion.Euler(rotation));
        }
    }

    public void Attack2(InputAction.CallbackContext context)
    {
        if (Time.time - attack2TimeStamp >= attack2Cooldown)
        {
            attack2TimeStamp = Time.time;

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

    public void TakeDamage(uint damageValue)
    {
        try
        {
            health = checked(health - damageValue);
        }
        catch
        {
            health = 0;
        }
    }
}
