using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public static GameObject[] players;

    // Start is called before the first frame update
    void Start()
    {
        if (!isServer) { return; }
        SpawnZombie(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer) { return; }
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void SpawnZombie(Vector3 spawnPos)
    {
        GameObject zombie = Instantiate(enemyPrefab);
        zombie.transform.position = spawnPos;
        EnemyController controller = zombie.GetComponent<EnemyController>();

        controller.modelID = Random.Range(0, controller.zombieModels.Length);

        NetworkServer.Spawn(zombie);
    }
}
