using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTransform : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    [Range(0.01f, 1f)]
    private float PositionThreshold;

    [SerializeField]
    [Range(0.01f, 1f)]
    private float RotationThreshold;

    [SerializeField]
    [Range(0.1f, 5f)]
    private float MovementInterpolationSpeed;

    [SerializeField]
    [Range(0.1f, 5f)]
    private float RotationInterpolationSpeed;

    private NetworkManager NetworkManager;
    private NetworkObject NetworkObject;

    [Header("Synced Data")]
    [SerializeField]
    private Vector3 SyncedPosition = new Vector3(-10, -10, -10);

    [SerializeField]
    private Vector3 SyncedRotation = new Vector3(-10, -10, -10);

    private Vector3 BufferedPosition;
    private Vector3 BufferedRotation;

    void Start()
    {
        NetworkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
        NetworkObject = transform.parent.GetComponent<NetworkObject>();
    }

    void Update()
    {
        if (NetworkObject.IsLocalPlayer)
        {
            if (Vector3.Distance(transform.position, SyncedPosition) > PositionThreshold)
            {
                string _newUri = URI.SendData.Replace("[SERVER-ID]", NetworkManager.ServerID.ToString());
                _newUri = _newUri.Replace("[PLAYER-ID]", NetworkManager.LocalPlayerID.ToString());
                _newUri = _newUri.Replace("[MODE]", "position");
                _newUri = _newUri.Replace("[DATA-TYPE]", "position");
                _newUri = _newUri.Replace("[DATA]", $"({transform.position.x}|{transform.position.y}|{transform.position.z})");
                BufferedPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                StartCoroutine(NetworkManager.GameplayWebRequest(_newUri, this));
            }

            if (Quaternion.Angle(Quaternion.Euler(transform.eulerAngles), Quaternion.Euler(SyncedRotation)) > RotationThreshold)
            {
                string _newUri = URI.SendData.Replace("[SERVER-ID]", NetworkManager.ServerID.ToString());
                _newUri = _newUri.Replace("[PLAYER-ID]", NetworkManager.LocalPlayerID.ToString());
                _newUri = _newUri.Replace("[MODE]", "rotation");
                _newUri = _newUri.Replace("[DATA-TYPE]", "rotation");
                _newUri = _newUri.Replace("[DATA]", $"({transform.eulerAngles.x}|{transform.eulerAngles.y}|{transform.eulerAngles.z})");
                BufferedRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                StartCoroutine(NetworkManager.GameplayWebRequest(_newUri, this));
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, SyncedPosition, MovementInterpolationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(SyncedRotation), RotationInterpolationSpeed * Time.deltaTime);
        }
    }

    public void LocalDataSynced()
    {
        SyncedPosition = BufferedPosition;
        SyncedRotation = BufferedRotation;
    }

    public void SyncData(Vector3 _position, Vector3 _rotation)
    {
        SyncedPosition = _position;
        SyncedRotation = _rotation;
    }
}
