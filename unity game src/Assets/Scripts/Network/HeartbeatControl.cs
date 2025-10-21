using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatControl : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    

    NetworkManager NetworkManager;

    void Start()
    {
        NetworkManager = GetComponent<NetworkManager>();
    }

    void Update()
    {
        
    }
}
