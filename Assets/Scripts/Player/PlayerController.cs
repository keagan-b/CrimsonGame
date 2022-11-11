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

    public float speed = 10f;

    private Animator characterAnimator;

    void Start()
    {
        if (!isLocalPlayer) { playerCam.enabled = false; playerCam.GetComponent<AudioListener>().enabled = false; }
        characterAnimator = characterModel.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }


        // movement handler
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // animation controls
        if (h != 0 || v != 0)
        {
            characterAnimator.SetFloat("Speed_f", 0.26f);
        }
        else
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