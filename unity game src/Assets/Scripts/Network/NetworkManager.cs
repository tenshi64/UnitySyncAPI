using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    [Header("Local Data")]
    public int LocalPlayerID = -1;
    public int ServerID = -1;
    public string ServerCode;
    public string Nickname;

    [SerializeField]
    public GameObject PlayerPrefab;

    [Header("Configuration")]
    [SerializeField]
    float TimeBeforeKick = 15; //time in seconds, before host kicks the player that doesnt send any heartbeat

    [SerializeField]
    private uint ClientUpdateRate = 30;

    [SerializeField]
    private float TickTimer;

    [SerializeField]
    private uint HeartbeatDelay = 15;

    [SerializeField]
    private float HeartbeatTimer;

    [Header("Synced Data")]
    public List<PlayerData> PlayerData;
    public Dictionary<int, GameObject> SpawnedPlayers = new Dictionary<int, GameObject>();
    private TextMeshProUGUI TextMeshLogs;
    private string JSONcontent;

    private void Start()
    {
        TextMeshLogs = GameObject.Find("Canvas").transform.Find("Logs").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (LocalPlayerID != -1 && ServerID != -1)
        {
            if (TickTimer >= 1 / ClientUpdateRate)
            {
                TickContent();
                if (HeartbeatTimer > HeartbeatDelay)
                {
                    SendHeartbeat();
                }

                if(PlayerData.Count > 0)
                {
                    if (PlayerData[0].ids.Count > 0)
                    {
                        for (int i = 0; i < PlayerData[0].ids.Count; i++)
                        {
                            if (!SpawnedPlayers.ContainsKey(int.Parse(PlayerData[0].ids[i])))
                            {
                                //player just connected

                                if(LocalPlayerID < int.Parse(PlayerData[0].ids[i])) //notify players who are already connected, about someone joining
                                {
                                    if (TextMeshLogs.text.Length > 0)
                                    {
                                        TextMeshLogs.text += $"\n{PlayerData[0].nicknames[i]} joined...";
                                    }
                                    else
                                    {
                                        TextMeshLogs.text = $"<b>Logs:</b> {PlayerData[0].nicknames[i]} joined...";
                                    }
                                }

                                SpawnedPlayers.Add(int.Parse(PlayerData[0].ids[i]), SpawnPlayer(int.Parse(PlayerData[0].ids[i]), EncodeVector3(PlayerData[0].positions[i]), EncodeVector3(PlayerData[0].rotations[i])));
                            }
                            else
                            {
                                if (PlayerData[0].ids.Contains(PlayerData[0].ids[i]))
                                {
                                    //there are some players connected, so theres data to sync
                                    SpawnedPlayers[int.Parse(PlayerData[0].ids[i])].GetComponentInChildren<NetworkTransform>().SyncData(EncodeVector3(PlayerData[0].positions[i]), EncodeVector3(PlayerData[0].rotations[i]));
                                    SpawnedPlayers[int.Parse(PlayerData[0].ids[i])].GetComponent<NetworkObject>().Nickname = PlayerData[0].nicknames[i];

                                    if((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - long.Parse(PlayerData[0].heartbeats[i]) >= TimeBeforeKick) //kick player when no heartbeat
                                    {
                                        string _newUri = URI.KickPlayer.Replace("[PLAYER-ID]", PlayerData[0].ids[i]);

                                        StartCoroutine(GameplayWebRequest(_newUri));
                                    }
                                }
                            }
                        }

                        foreach (KeyValuePair<int, GameObject> _playerData in SpawnedPlayers.ToList()) //check if there's someone that disconnected
                        {
                            if(!PlayerData[0].ids.Contains(_playerData.Key.ToString()))
                            {
                                GameObject _toDelete = SpawnedPlayers[_playerData.Value.GetComponent<NetworkObject>().PlayerID];
                                SpawnedPlayers.Remove(_toDelete.GetComponent<NetworkObject>().PlayerID);

                                if (TextMeshLogs.text.Length > 0)
                                {
                                    TextMeshLogs.text += $"\n{_toDelete.GetComponent<NetworkObject>().Nickname} left...";
                                }
                                else
                                {
                                    TextMeshLogs.text = $"<b>Logs:</b> {_toDelete.GetComponent<NetworkObject>().Nickname} left...";
                                }

                                if (LocalPlayerID == _toDelete.GetComponent<NetworkObject>().PlayerID)
                                {
                                    Application.Quit();
                                }
                                Destroy(_toDelete);
                            }
                        }
                    }
                }
                else
                {
                    //no ppl to sync data with, just one player left
                    if(SpawnedPlayers.Count > 0)
                    {
                        foreach(KeyValuePair<int, GameObject> _playerData in SpawnedPlayers.ToList())
                        {
                            GameObject _toDelete = SpawnedPlayers[_playerData.Value.GetComponent<NetworkObject>().PlayerID];
                            SpawnedPlayers.Remove(_toDelete.GetComponent<NetworkObject>().PlayerID);

                            if (TextMeshLogs.text.Length > 0)
                            {
                                TextMeshLogs.text += $"\n{_toDelete.GetComponent<NetworkObject>().Nickname} left...";
                            }
                            else
                            {
                                TextMeshLogs.text = $"<b>Logs:</b> {_toDelete.GetComponent<NetworkObject>().Nickname} left...";
                            }

                            if (LocalPlayerID == _toDelete.GetComponent<NetworkObject>().PlayerID)
                            {
                                Application.Quit();
                            }
                            Destroy(_toDelete);
                        }
                    }
                }

                HeartbeatTimer += Time.deltaTime;
                TickTimer = 0;
            }
            TickTimer += Time.deltaTime;
        }
    }

    void SendHeartbeat()
    {
        HeartbeatTimer = 0;
        string _newUri = URI.SendHeartbeat.Replace("[SERVER-ID]", ServerID.ToString());
        _newUri = _newUri.Replace("[PLAYER-ID]", LocalPlayerID.ToString());
        StartCoroutine(GameplayWebRequest(_newUri));
    }

    private void TickContent()
    {
        //get every possible data from server about lobby
        string _newUri = URI.ReceiveData.Replace("[SERVER-ID]", ServerID.ToString());
        _newUri = _newUri.Replace("[PLAYER-ID]", LocalPlayerID.ToString());

        StartCoroutine(GameplayWebRequest(_newUri));
    }

    public IEnumerator GameplayWebRequest(string _uri, NetworkTransform _networkTransform = null)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(_uri);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + webRequest.error);
        }
        else
        {
            JSONcontent = webRequest.downloadHandler.text;

            if (URI.CompareURIs(_uri, URI.SendHeartbeat))
            {
                Debug.Log("Heartbeat sent");
            }
            else if (URI.CompareURIs(_uri, URI.ReceiveData))
            {
                if(JSONcontent != "null")
                {
                    var _fromJson = JsonUtility.FromJson<PlayerData>(JSONcontent);

                    PlayerData = new List<PlayerData> { _fromJson };
                }
                else
                {
                    PlayerData = new List<PlayerData>();
                }
            }
            else if (URI.CompareURIs(_uri, URI.SendData))
            {
                if(JSONcontent == "ok")
                {
                    if (_networkTransform != null)
                    {
                        _networkTransform.LocalDataSynced();
                    }
                }
            }
        }
    }

    public GameObject SpawnPlayer(int _playerID, Vector3 _initialPosition, Vector3 _initialRotation)
    {
        GameObject _player = Instantiate(PlayerPrefab, _initialPosition, Quaternion.Euler(_initialRotation), null);
        _player.GetComponent<NetworkObject>().PlayerID = _playerID;

        return _player;
    }

    public Vector3 EncodeVector3(string _stringVector3)
    {
        int _firstBracket = -1;
        int _lastBracket = -1;
        int _firstSeparator = -1;
        int _lastSeparator = -1;
        for (int i = 0; i < _stringVector3.Length; i++)
        {
            if (_stringVector3[i] == '(')
            {
                _firstBracket =i;
            }
            else if (_stringVector3[i] == ')')
            {
                _lastBracket = i;
            }
            else if (_stringVector3[i] == '|')
            {
                if (_firstSeparator == -1)
                {
                    _firstSeparator = i;
                }
                else
                {
                    _lastSeparator = i;
                }
            }
        }

        string _firstNumber = _stringVector3.Substring(_firstBracket+1, _firstSeparator - _firstBracket - 1);
        string _secondNumber = _stringVector3.Substring(_firstSeparator+1, _lastSeparator - _firstSeparator - 1);
        string _thirdNumber = _stringVector3.Substring(_lastSeparator+1, _lastBracket - _lastSeparator - 1);

        if(_firstNumber.Length == 0)
        {
            _firstNumber = "0";
        }
        if (_secondNumber.Length == 0)
        {
            _secondNumber = "0";
        }
        if (_thirdNumber.Length == 0)
        {
            _thirdNumber = "0";
        }
        return new Vector3(float.Parse(_firstNumber), float.Parse(_secondNumber), float.Parse(_thirdNumber));
    }
}

[System.Serializable]
public class PlayerData
{
    public List<string> ids;
    public List<string> nicknames;
    public List<string> positions;
    public List<string> rotations;
    public List<string> heartbeats;
}