using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.ComponentModel;

public class GameManager : NetworkBehaviour
{
    [HideInInspector]
    public static GameManager singleton;

    [Header("Object Tracking")]
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
    public GameObject gameUI;

    [Space]
    [Header("Store Information")]
    [SyncVar]
    public bool isInStore = false;
    public StoreHandler storeHandler;
    
    public GameObject storeUI;

    [Space]
    [Header("Game Options")]
    private int remainingSpawns = 1;
    private float spawnSpeed = 1f;
    private float spawnCooldown = 0f;

    public float roundInterval = 5f;
    private float roundCooldown = 0f;

    // set singleton
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
        if (roundCooldown >= Time.time || !isServer)
        {
            return;
        }

        players = GameObject.FindGameObjectsWithTag("Player");
        livingPlayers = GetLivingPlayers();

        if (isInStore) { return; }

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
    public void RoundEnd(bool wasPlayerWin)
    {
        if (wasPlayerWin)
        {
            ReviveAll();
            round++;
            roundCooldown = Time.time + roundInterval;
            remainingSpawns = (int)Mathf.Ceil(UnityEngine.Random.Range(1f, 2.5f) * round);
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

        // starts the shop
        if (round % 2 == 0)
        {
            inRound = false;
            isInStore = true;
            storeHandler.RpcGenerateWeapons();
            RpcSetStoreCamera(true);
        }
    }

    // spawns zombies
    public void SpawnZombie()
    {
        GameObject zombie = Instantiate(enemyPrefab);
        zombie.transform.position = zombieSpawners[UnityEngine.Random.Range(0, zombieSpawners.Length)].transform.position;
        EnemyController controller = zombie.GetComponent<EnemyController>();

        controller.modelID = UnityEngine.Random.Range(0, controller.zombieModels.Length);
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
        isInStore = false;
        inRound = true;
        RpcSetStoreCamera(false);
    }

    [ClientRpc]
    public void RpcSetStoreCamera(bool state)
    {
        //storeHandler.storeCam.enabled = state;
        if (state) {

            PlayerController pc = NetworkClient.localPlayer.GetComponent<PlayerController>();
            pc.playerCam.transform.position = storeHandler.storeCamLocation.transform.position;
            pc.playerCam.transform.rotation = storeHandler.storeCamLocation.transform.rotation;
        }
        else
        {
            PlayerController pc = NetworkClient.localPlayer.GetComponent<PlayerController>();
            pc.playerCam.transform.position = pc.playerCamLocation.transform.position;
            pc.playerCam.transform.rotation = pc.playerCamLocation.transform.rotation;
        }
        gameUI.SetActive(!state);
        storeUI.SetActive(state);
    }
}
