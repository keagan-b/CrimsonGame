using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MainScreenController : MonoBehaviour
{
    NetworkManager networkManager;
    
    public TMP_InputField joinIP, joinPort, hostPort;
    public TextMeshProUGUI joinPortError, hostPortError;


    private void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void UpdateJoinInformation()
    {
        networkManager.networkAddress = joinIP.text;

        ushort port;
        if (ushort.TryParse(joinPort.text, out port))
        {
            networkManager.GetComponent<kcp2k.KcpTransport>().Port = port;
        }
        else
        {
            joinPortError.enabled = true;
            return;
        }
    }

    public void UpdateHostPort()
    {
        ushort port;
        if (ushort.TryParse(hostPort.text, out port))
        {
            networkManager.GetComponent<kcp2k.KcpTransport>().Port = port;
        }
        else
        {
            hostPortError.enabled = true;
            return;
        }
    }

    public void QuitGame()
    {
        Application.Quit();   
    }
}
