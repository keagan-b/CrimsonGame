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
    public GameObject raycastPoint;
    public GameObject bulletPrefab;

    public float speed = 1f;

    private Animator characterAnimator;

    [SyncVar]
    public float health = 100f;
    [SyncVar]
    public bool isDead = false;

    private int spectatorCamera = 0;

    public float attackSpeed = 0.3f;
    private float attackCooldown = 0f;

    void Start()
    {
        if (!isLocalPlayer) { playerCam.enabled = false; playerCam.GetComponent<AudioListener>().enabled = false; }

        characterAnimator = characterModel.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }

        if (health <= 0 && !isDead) // death
        {
            isDead = true;

            characterAnimator.SetBool("Death_b", true);
            characterAnimator.SetInteger("DeathType_int", Random.Range(1, 3));
            playerCam.enabled = false;
            return;
        }
        else if (isDead && health >= 100) // revival
        {
            isDead = false;
            characterAnimator.SetBool("Death_b", false);
            characterAnimator.Play("Alive");
            GameObject[] players = GameManager.players;
            foreach (GameObject player in players)
            {
                PlayerController controller = player.GetComponent<PlayerController>();
                controller.playerCam.enabled = false;
            }
        }
        else if (isDead && health <= 0) // spectator system
        {
            GameObject[] players = GameManager.livingPlayers;
            int prevCamera = spectatorCamera;

            if (Input.GetKeyDown(KeyCode.A))
            {
                spectatorCamera--;
                if (spectatorCamera < 0) { spectatorCamera = players.Length - 1; }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                spectatorCamera++;
                if (spectatorCamera > players.Length - 1) { spectatorCamera = 0; }
            }
            else
            {
                return;
            }

            players[prevCamera].GetComponent<PlayerController>().playerCam.enabled = false;
            players[spectatorCamera].GetComponent<PlayerController>().playerCam.enabled = true;

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

        // attack handler
        if (Input.GetMouseButton(0)) 
        {
            if (attackCooldown <= Time.time)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.transform.position = raycastPoint.transform.position;

                bullet.GetComponent<Rigidbody>().AddForce(new Vector3(hit.point.x, bullet.transform.position.y, hit.point.z) * 750);

                NetworkServer.Spawn(bullet);
                attackCooldown = Time.time + attackSpeed;
            }
        }
    }
}