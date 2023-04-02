using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class UIController : NetworkBehaviour
{
    public TextMeshProUGUI roundText, zombiesRemaining, gold;
    public TMP_InputField nameField;
    public GameObject deathScreen, aliveScreen, pauseScreen, localPlayer;

    public static UIController singleton;

    PlayerController playerController;
    void Start()
    {
        if (singleton != this)
        {
            singleton = this;
            localPlayer = NetworkClient.localPlayer.gameObject;
            playerController = localPlayer.GetComponent<PlayerController>();
        }
    }

    public void Disconnect()
    {
        if (isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    public void SetName()
    {
        Debug.Log("running set name!");
        CmdSetName(nameField.text.Substring(0, Mathf.Min(nameField.text.Length, 24)), localPlayer);
    }

    [Command(requiresAuthority=false)]
    void CmdSetName(string playerName, GameObject player)
    {
        Debug.Log($"Setting name to {playerName}");
        player.name = playerName;
        player.GetComponent<PlayerController>().playerName = playerName;
    }

    // Update is called once per frame
    void Update()
    {
        gold.text = $"{playerController.gold} Gold";
        roundText.text = "Round " + (GameManager.singleton.round + 1);
        zombiesRemaining.text = GameManager.singleton.zombies.Count + " Left";

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseScreen.activeInHierarchy)
            {
                pauseScreen.SetActive(false);
            }
            else
            {
                pauseScreen.SetActive(true);
            }
        }
    }
}
