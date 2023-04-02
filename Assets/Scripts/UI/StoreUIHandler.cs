using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreUIHandler : MonoBehaviour
{
    public Button item1Button, item2Button, item3Button;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buyItem1()
    {
        buyItem(GameManager.singleton.storeHandler.items[0]);
    }

    public void buyItem2()
    {
        buyItem(GameManager.singleton.storeHandler.items[1]);
    }

    public void buyItem3()
    {
        buyItem(GameManager.singleton.storeHandler.items[2]);
    }

    void buyItem(GameObject item)
    {
        WeaponObj weaponInfo = item.GetComponent<WeaponObj>();
        PlayerController pc = NetworkClient.localPlayer.GetComponent<PlayerController>();
        if (pc.gold < weaponInfo.weapon.price)
        {
            return;
        }
        else
        {
            pc.gold -= weaponInfo.weapon.price;
            pc.weaponObj = weaponInfo;
        }


    }
}
