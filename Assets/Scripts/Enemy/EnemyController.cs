using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class EnemyController : NetworkBehaviour
{
    public GameObject modelParent;
    public NavMeshAgent navAgent;
    public GameObject[] zombieModels;
    public int modelID;
    public float damage = 10f;

    [SyncVar]
    public float health = 100;

    public float attackSpeed = 1f;

    private float attackCooldown = 0f;
    private Animator animator;

    private void Start()
    {
        GameObject model = zombieModels[modelID];
        model = Instantiate(model, modelParent.transform);
        model.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        animator = model.GetComponent<Animator>();
        NetworkAnimator netAnim = gameObject.AddComponent<NetworkAnimator>();
        netAnim.animator = animator;
        netAnim.clientAuthority = true;
        netAnim.syncDirection = SyncDirection.ClientToServer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isServer) { return; }

        if (health <= 0)
        {
            GameManager.zombies.Remove(gameObject);
            Destroy(gameObject);
        }

        // find nearest player
        float min = float.MaxValue;
        Vector3 closestPlayer = new Vector3();
        foreach (GameObject player in GameManager.players)
        {
            float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);
            if (distance < min)
            {
                min = distance;
                closestPlayer = player.transform.position;
            }
        }

        navAgent.SetDestination(closestPlayer);

        if (min > 1)
        {
            gameObject.transform.LookAt(closestPlayer);
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!isServer) { return; }
        if (collision.gameObject.tag == "Player" && attackCooldown <= Time.time)
        {
            Debug.Log("Attacking!");
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            controller.health -= damage;
            attackCooldown = Time.time + attackSpeed;

            
            animator.SetTrigger("Attack");
        }
    }
}