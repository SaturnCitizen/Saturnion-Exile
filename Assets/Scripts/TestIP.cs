using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine;


public class TestIP : MonoBehaviour {

    /// <summary>
    /// Function used to get informations about the server and mainly to know the IP address 
    /// of the server to share it with other users and let them connect
    /// </summary>


    void OnGUI()
    {
        string ipadress = Network.player.ipAddress;
        GUI.Box(new Rect(10, Screen.height - 50, 100, 50), ipadress);
        GUI.Label(new Rect(20, Screen.height - 35, 100, 20), "Status: " + NetworkServer.active);
        GUI.Label(new Rect(20, Screen.height - 20, 100, 20), "Connected: " + NetworkServer.connections.Count);
    }

}
