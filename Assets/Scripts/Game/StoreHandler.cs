using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreHandler : NetworkBehaviour
{
    public GameObject storeCamLocation;
    public WeaponDatabase weaponDB;
    public List<GameObject> spawners;

    [HideInInspector]
    public GameObject[] items;

    private void Start()
    {
        items = new GameObject[spawners.Count];
    }

    // fills all spawners with a weapon
    [ClientRpc]
    public void RpcGenerateWeapons()
    {
        for (int i = 0; i < spawners.Count; i++)
        {
            Debug.Log("spawning weapon....");
            GameObject newWeapon = Instantiate(weaponDB.RandomWeapon(), spawners[i].transform);
            Weapon weapon = newWeapon.GetComponent<WeaponObj>().weapon;
            // randomize weapon damage
            weapon.damage = weapon.baseDamage + Random.Range(-weapon.damageRange, weapon.damageRange);
            weapon.price = 0;
            items[i] = newWeapon;
        }
    }
}
