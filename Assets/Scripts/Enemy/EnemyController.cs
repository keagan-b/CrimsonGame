using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class EnemyController : NetworkBehaviour
{
    public GameObject modelParent;
    public NavMeshAgent navAgent;
    public NetworkAnimator netAnimator;
    public GameObject[] zombieModels;
    public int modelID;

    [SyncVar]
    public float health = 100;

    private void Start()
    {
        GameObject model = zombieModels[modelID];
        model = Instantiate(model, modelParent.transform);
        model.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        NetworkAnimator netAnim = gameObject.AddComponent<NetworkAnimator>();
        netAnim.animator = model.GetComponent<Animator>();
        netAnim.clientAuthority = true;
        netAnim.syncDirection = SyncDirection.ClientToServer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isServer) { return; }
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
}
