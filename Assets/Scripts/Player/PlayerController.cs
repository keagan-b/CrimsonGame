using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public Camera playerCam;
    public CharacterController characterController;
    public GameObject modelParent;
    public GameObject characterModel;

    public float speed = 1f;

    private Animator characterAnimator;

    [SyncVar]
    public float health = 100f;
    [SyncVar]
    public bool isDead = false;

    void Start()
    {
        if (!isLocalPlayer) { playerCam.enabled = false; playerCam.GetComponent<AudioListener>().enabled = false; }

        characterAnimator = characterModel.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }

        if (health <= 0 && !isDead)
        {
            isDead = true;

            characterAnimator.SetBool("Death_b", true);
            characterAnimator.SetInteger("DeathType_int", Random.Range(1, 3));
            playerCam.enabled = false;
            return;
        }
        else if (isDead && health >= 100)
        {
            characterAnimator.SetBool("Death_b", false);
        }

        // movement handler
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // animation controls
        if ((h != 0 || v != 0) && characterAnimator != null)
        {
            characterAnimator.SetFloat("Speed_f", 0.26f);
        }
        else if (characterAnimator != null)
        {
            characterAnimator.SetFloat("Speed_f", 0f);
        }

        characterController.Move(new Vector3((h * speed), 0, (v * speed)));


        // rotation handler (faces mouse cursor)
        Vector3 mousePos = Input.mousePosition;
        Ray ray = playerCam.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            float angle = Mathf.Atan2(hit.point.x, hit.point.z) * Mathf.Rad2Deg;
            modelParent.transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }
}