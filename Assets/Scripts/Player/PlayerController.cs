using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public Camera playerCam;
    public CharacterController characterController;
    public GameObject modelParent;
    public GameObject characterModel;
    public GameObject raycastPoint;
    public GameObject bulletPrefab;
    public TextMeshPro nametag;
    public HealthBarController healthBar;

    public float speed = 1f;

    private Animator characterAnimator;

    [SyncVar]
    public float health = 100f;
    [SyncVar]
    public bool isDead = false;
    [SyncVar]
    public string playerName = "unnamed";

    private int spectatorCamera = 0;

    public float damage = 100f;
    public float attackSpeed = 0.3f;
    private float attackCooldown = 0f;

    void Start()
    {
        if (!isLocalPlayer) { playerCam.enabled = false; playerCam.GetComponent<AudioListener>().enabled = false; }

        characterAnimator = characterModel.GetComponent<Animator>();
        healthBar.fullHealth = health;
    }

    void FixedUpdate()
    {
        healthBar.SetHealth(health);

        if (nametag.text != playerName)
        {
            Debug.Log("Changing Name!");
            nametag.text = playerName;
        }

        if (!isLocalPlayer) { return; }

        if (health <= 0 && !isDead) // death
        {
            CmdMarkDead();
            return;
        }
        else if (isDead && health <= 0) // spectator system
        {
            GameObject[] players = GameManager.singleton.livingPlayers;
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

            return;
        }

        // movement handler
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // animation controls
        if ((h != 0 || v != 0) && characterAnimator != null)
        {
            CmdWalkAnimation();
        }
        else if (characterAnimator != null)
        {
            CmdIdleAnimation();
        }

        characterController.Move(new Vector3((h * speed), 0, (v * speed)));


        // rotation handler (faces mouse cursor)
        Vector3 mousePos = Input.mousePosition;
        Ray ray = playerCam.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            modelParent.transform.LookAt(new Vector3(hit.point.x, modelParent.transform.position.y, hit.point.z));
        }

        // attack handler
        if (Input.GetMouseButton(0))
        {
            if (attackCooldown <= Time.time)
            {
                CmdSpawnBullet(hit.point, raycastPoint.transform.position, damage);
                attackCooldown = Time.time + attackSpeed;
            }
        }
    }

    public void Revive(float reviveHealth)
    {
        isDead = false;
        health = reviveHealth;
        characterAnimator.SetBool("Death_b", false);
        characterAnimator.Play("Alive");

        characterAnimator.transform.localPosition = new Vector3();

        GetComponent<CharacterController>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;

        if (!isLocalPlayer) { return; }

        GameObject[] players = GameManager.singleton.livingPlayers;
        foreach (GameObject player in players)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.playerCam.enabled = false;
        }
        playerCam.enabled = true;
    }

    [Command]
    void CmdSpawnBullet(Vector3 hitPoint, Vector3 raycast, float damage)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = raycast;

        BulletHandler bh = bullet.GetComponent<BulletHandler>();

        bh.damage = damage;
        bh.target = new Vector3(hitPoint.x, raycast.y, hitPoint.z);

        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdMarkDead()
    {
        isDead = true;

        GetComponent<CharacterController>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;

        characterAnimator.SetBool("Death_b", true);
        characterAnimator.SetInteger("DeathType_int", Random.Range(1, 3));
    }

    [Command]
    void CmdWalkAnimation()
    {
        characterAnimator.SetFloat("Speed_f", 0.26f);
    }

    [Command]
    void CmdIdleAnimation()
    {
        characterAnimator.SetFloat("Speed_f", 0f);
    }
}