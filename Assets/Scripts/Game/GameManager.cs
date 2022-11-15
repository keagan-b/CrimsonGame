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

    [SyncVar]
    public bool inRound = true;
    [SyncVar]
    public bool inStore = false;

    private int remainingSpawns = 1;
    private float spawnSpeed = 1f;
    private float spawnCooldown = 0f;

    public float roundInterval = 5f;
    private float roundCooldown = 0f;

    void Start()
    {
        if (singleton != this)
        {
            singleton = this;
        }
        if (!isServer) { return; }
    }

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

        if (remainingSpawns > 0 && inRound)
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
            ReviveAll();
            round++;
            roundCooldown = Time.time + roundInterval;
            remainingSpawns = (int)Mathf.Ceil(Random.Range(1f, 2.5f) * round);
        }
        else
        {
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

        if (round % 5 == 0)
        {
            inRound = false;
            inStore = true;
        }
    }

    // spawns zombies
    public void SpawnZombie()
    {
        GameObject zombie = Instantiate(enemyPrefab);
        zombie.transform.position = zombieSpawners[Random.Range(0, zombieSpawners.Length)].transform.position;
        EnemyController controller = zombie.GetComponent<EnemyController>();

        controller.modelID = Random.Range(0, controller.zombieModels.Length);
        if (round > 1)
        {
            controller.health = (float)(100 * round * 0.025);
        }

        zombies.Add(zombie);

        NetworkServer.Spawn(zombie);
    }

    // causes revives
    public void ReviveAll()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().Revive(100f);
        }
    }

    // gets an array of all living players
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

    // triggers when majority has voted to end the store phase
    public void EndStore()
    {
        inStore = false;
        inRound = true;
    }
}
