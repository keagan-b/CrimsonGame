using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class UIController : NetworkBehaviour
{
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI zombiesRemaining;
    public TMP_InputField nameField;
    public GameObject deathScreen, aliveScreen, pauseScreen;

    public static UIController singleton;
    void Start()
    {
        if (singleton != this)
        {
            singleton = this;
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
        Debug.Log("running name set!");
        CmdSetName(nameField.text.Substring(0, Mathf.Min(nameField.text.Length, 24)), NetworkClient.localPlayer.gameObject);
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
        roundText.text = "Round " + GameManager.singleton.round;
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
