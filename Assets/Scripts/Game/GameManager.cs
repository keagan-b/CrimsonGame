using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [HideInInspector]
    public static GameManager singleton;

    public GameObject enemyPrefab;

    public GameObject[] players;

    public GameObject[] livingPlayers;

    public GameObject[] zombieSpawners;

    [SyncVar]
    public List<GameObject> zombies = new List<GameObject>();

    [SyncVar]
    public int round = 1;

    private int remainingSpawns = 1;
    private float spawnSpeed = 1f;
    private float spawnCooldown = 0f;

    public float roundInterval = 5f;
    private float roundCooldown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (singleton != this)
        {
            singleton = this;
        }
        if (!isServer) { return; }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        livingPlayers = GetLivingPlayers();

        if (roundCooldown >= Time.time || !isServer)
        {
            return;
        }

        bool allDead = true;
        foreach (GameObject player in players)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (!controller.isDead) { allDead = false; break; }
        }

        if (remainingSpawns > 0)
        {
            if (spawnCooldown <= Time.time)
            {
                SpawnZombie();
                spawnCooldown = spawnSpeed + Time.time;
                remainingSpawns--;
            }
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

            ReviveAll();
            round++;
            roundCooldown = Time.time + roundInterval;
            remainingSpawns = (int)Mathf.Ceil(Random.Range(1f, 2.5f) * round);
        }
        else
        {
            Debug.Log("Game over.");
            foreach (GameObject zombie in zombies)
            {
                Destroy(zombie);
            }
            ReviveAll();
            zombies.Clear();

            round = 1;
            remainingSpawns = 1;
            roundCooldown = Time.time + roundInterval;
        }
    }

    public void SpawnZombie()
    {
        GameObject zombie = Instantiate(enemyPrefab);
        zombie.transform.position = zombieSpawners[Random.Range(0, zombieSpawners.Length)].transform.position;
        EnemyController controller = zombie.GetComponent<EnemyController>();

        controller.modelID = Random.Range(0, controller.zombieModels.Length);

        zombies.Add(zombie);

        NetworkServer.Spawn(zombie);
    }

    public void ReviveAll()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().Revive(100f);
        }
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
