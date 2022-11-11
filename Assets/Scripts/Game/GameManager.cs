using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public static GameObject[] players;
    public static GameObject[] livingPlayers;

    public static List<GameObject> zombies = new List<GameObject>();
    public static Camera levelCamera;

    [SyncVar]
    public int round = 0;
    [SyncVar]
    public int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!isServer) { return; }
        SpawnZombie(Vector3.zero);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isServer) { return; }
        players = GameObject.FindGameObjectsWithTag("Player");
        livingPlayers = GetLivingPlayers();

        bool allDead = true;
        foreach (GameObject player in players)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (!controller.isDead) { allDead = false; break; }
        }

        if (allDead)
        {
            RoundEnd(false);
        }

        if (zombies.Count <= 0)
        {
            RoundEnd(true);
        }
    }

    // handle round resets
    public void RoundEnd(bool playerWin)
    {
        if (playerWin)
        {
            Debug.Log("Round finished!");
            score++;
            foreach (GameObject player in players)
            {
                PlayerController controller = player.GetComponent<PlayerController>();
                controller.health = 100f;
                controller.isDead = false;
            }
        }
        else
        {
            Debug.Log("Game over.");
            foreach (GameObject zombie in zombies)
            {
                Destroy(zombie);
                score = 0;
                round = 0;
            }
        }
    }

    public void SpawnZombie(Vector3 spawnPos)
    {
        GameObject zombie = Instantiate(enemyPrefab);
        zombie.transform.position = spawnPos;
        EnemyController controller = zombie.GetComponent<EnemyController>();

        controller.modelID = Random.Range(0, controller.zombieModels.Length);

        zombies.Add(zombie);

        NetworkServer.Spawn(zombie);
    }

    public GameObject[] GetLivingPlayers()
    {
        List<GameObject> livingPlayers = new List<GameObject>();
        foreach (GameObject player in players)
        {
            if (!player.GetComponent<PlayerController>().isDead)
            {
                livingPlayers.Add(player);
            }
        }

        return livingPlayers.ToArray();
    }
}
