using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName ="Databases/Weapons", order = 1)]
public class WeaponDatabase : ScriptableObject
{
    [SerializeField]
    List<GameObject> weapons;

    // returns a prefab by ID (index in list)
    public GameObject GameObjectByID(int id)
    {
        GameObject weaponObj = weapons[id];
        WeaponObj weapon = weaponObj.GetComponent<WeaponObj>();
        weapon.weapon.damage = weapon.weapon.baseDamage + Random.Range(-weapon.weapon.damageRange, weapon.weapon.damageRange);
        return weaponObj;
    }

    // gets a weapon by ID (index in list)
    public WeaponObj WeaponByID(int id)
    {
        WeaponObj weapon = weapons[id].GetComponent<WeaponObj>();
        weapon.weapon.damage = weapon.weapon.baseDamage + Random.Range(-weapon.weapon.damageRange, weapon.weapon.damageRange);
        return weapon;
    }

    // returns a random weapon from the database
    public GameObject RandomWeapon()
    {
        int id = Random.Range(0, weapons.Count);
        weapons[id].GetComponent<WeaponObj>().weapon.id = id;
        return weapons[id];
    }
}
