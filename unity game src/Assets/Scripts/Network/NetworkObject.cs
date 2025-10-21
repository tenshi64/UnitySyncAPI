using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObject : MonoBehaviour
{
    [Header("Synced Data")]
    public int PlayerID = 0;
    public bool IsLocalPlayer = false;
    public string Nickname;
    private NetworkManager NetworkManager;

    void Start()
    {
        NetworkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        if(NetworkManager.LocalPlayerID == PlayerID)
        {
            IsLocalPlayer = true;
            Nickname = NetworkManager.Nickname;
        }
        else
        {
            IsLocalPlayer = false;
        }
    }
}
