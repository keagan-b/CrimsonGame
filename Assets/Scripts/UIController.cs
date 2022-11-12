using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI zombiesRemaining;
    public GameObject deathScreen, aliveScreen;

    public static UIController singleton;
    void Start()
    {
        if (singleton != this)
        {
            singleton = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        roundText.text = "Round " + GameManager.singleton.round;
        zombiesRemaining.text = GameManager.singleton.zombies.Count + " Left";
    }
}
