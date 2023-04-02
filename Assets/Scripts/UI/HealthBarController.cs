using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthBarController : MonoBehaviour
{
    public GameObject healthBar;
    public GameObject healthBarBG;
    public float fullHealth = 100f;


    private float defaultBarScale;
    private void Start()
    {
        defaultBarScale = healthBar.transform.localScale.x;
    }

    public void SetHealth(float health)
    {
        if (health < fullHealth)
        {
            float newX = 0f;
            if (health > 0)
            {
                newX = (defaultBarScale / fullHealth) * health;
            }
            healthBar.transform.localScale = new Vector3(newX, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
            healthBar.SetActive(true);
            healthBarBG.SetActive(true);
        }
        else
        {
            healthBar.SetActive(false);
            healthBarBG.SetActive(false);
        }
    }
}