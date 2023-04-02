using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    [SerializeField]
    public string weaponName;
    public bool isTwoHanded = false;

    [Space]
    [Header("Damage Settings")]
    [HideInInspector]
    public float damage = 0f;

    public float baseDamage = 0f;

    public float damageRange = 0f;

    public float fireRate = 1f;

    [HideInInspector]
    public int id;

    [HideInInspector]
    public int price = 0;
}

public class WeaponObj : MonoBehaviour
{
    public Weapon weapon;
}
