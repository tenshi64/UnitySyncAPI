using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private GameObject JoinButtonPrefab;
    public Coroutine RequestCoroutine;
    private string JSONcontent;
    private NetworkManager NetworkManager;

    void Start()
    {
        NetworkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
        SendRequest(URI.LobbyNames);
    }

    public void SendRequest(string _uri)
    {
        if(RequestCoroutine == null)
        {
            RequestCoroutine = StartCoroutine(LobbyWebRequest(_uri));
        }
    }

    private void DestroyExistingButtons()
    {
        foreach(var _button in transform.Find("Lobby Browser").Find("Viewport").Find("Content").gameObject.GetComponentsInChildren<Button>())
        {
            Destroy(_button.gameObject);
        }
    }

    public IEnumerator LobbyWebRequest(string _uri)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(_uri);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + webRequest.error);
            RequestCoroutine = null;
        }
        else
        {
            JSONcontent = webRequest.downloadHandler.text;

            if(_uri == URI.LobbyNames)
            {
                RequestCoroutine = null;
                DestroyExistingButtons();
                SpawnLobbyButtons();
            }
            else if(URI.CompareURIs(_uri, URI.HostLobby))
            {
                if (JSONcontent != "failed")
                {
                    Debug.Log("hosted");
                    var _fromJson = JsonUtility.FromJson<HostData>(JSONcontent);

                    string _newUri = URI.JoinLobby.Replace("[SERVER-CODE]", NetworkManager.ServerCode);
                    _newUri = _newUri.Replace("[SERVER-ID]", _fromJson.server_id);
                    _newUri = _newUri.Replace("[NICKNAME]", NetworkManager.Nickname);

                    transform.parent.Find("Loading Screen").gameObject.SetActive(true);

                    RequestCoroutine = null;
                    SendRequest(_newUri);
                }
                else
                {
                    RequestCoroutine = null;
                    Debug.Log("failed");
                    //something went wrong
                }
            }
            else if (URI.CompareURIs(_uri, URI.JoinLobby))
            {
                if (JSONcontent == "-1")
                {
                    RequestCoroutine = null;
                    Debug.Log("the code is invalid");
                    transform.Find("Lobby Code").Find("Error Message").GetComponent<TextMeshProUGUI>().text = "The code is incorrect.";
                    transform.Find("Lobby Code").Find("Error Message").gameObject.SetActive(true);
                }
                else if(JSONcontent == "failed")
                {
                    RequestCoroutine = null;
                    Debug.Log("Something went wrong");
                    transform.Find("Lobby Code").Find("Error Message").GetComponent<TextMeshProUGUI>().text = "Failed to connect to server.";
                    transform.Find("Lobby Code").Find("Error Message").gameObject.SetActive(true);
                }
                else
                {
                    //joined
                    var _fromJson = JsonUtility.FromJson<InitialJoinData>(JSONcontent);

                    NetworkManager.LocalPlayerID = int.Parse(_fromJson.player_id);
                    NetworkManager.ServerID = int.Parse(_fromJson.server_id);
                    NetworkManager.ServerCode = _fromJson.server_code;
                    NetworkManager.Nickname = _fromJson.nick;

                    Debug.Log("the code is valid");
                    transform.parent.Find("Loading Screen").gameObject.SetActive(false);
                    transform.parent.Find("Lobby Code Info").GetComponent<TextMeshProUGUI>().text = $"<b>Lobby code</b>: {NetworkManager.ServerCode}";
                    transform.parent.Find("Lobby Code Info").gameObject.SetActive(true);
                    transform.Find("Lobby Code").Find("Error Message").gameObject.SetActive(false);

                    RequestCoroutine = null;
                    NetworkManager.SpawnPlayer(NetworkManager.LocalPlayerID, new Vector3(0, 32.14137f, 0), Vector3.zero);

                    gameObject.SetActive(false);
                }
            }
            else if (URI.CompareURIs(_uri, URI.GenerateLobbyCode))
            {
                Debug.LogError(JSONcontent);
                NetworkManager.ServerCode = JSONcontent;
                NetworkManager.Nickname = transform.Find("Lobby Browser").Find("Nickname").GetComponent<TMP_InputField>().text;

                string _newUri = URI.HostLobby.Replace("[SERVER-NAME]", transform.Find("Lobby Browser").Find("Lobby Name").GetComponent<TMP_InputField>().text);
                _newUri = _newUri.Replace("[SERVER-CODE]", JSONcontent);

                transform.parent.Find("Loading Screen").gameObject.SetActive(true);

                RequestCoroutine = null;
                SendRequest(_newUri);
            }
        }

        RequestCoroutine = null;
    }

    private void SpawnLobbyButtons()
    {
        if(JSONcontent != "null")
        {
            var _fromJson = JsonUtility.FromJson<Server>(JSONcontent);

            if (_fromJson.names.Count > 0)
            {
                transform.Find("Lobby Browser").Find("Viewport").Find("Content").Find("No Lobbies Text").gameObject.SetActive(false);
                for (int i = 0; i < _fromJson.names.Count; i++)
                {
                    Transform _button = Instantiate(JoinButtonPrefab, transform.Find("Lobby Browser").Find("Viewport").Find("Content")).transform;
                    _button.Find("Server Name").GetComponent<TextMeshProUGUI>().text = _fromJson.names[i];
                    _button.GetComponent<Buttons>().ServerID = int.Parse(_fromJson.ids[i]);
                }
            }
            else
            {
                transform.Find("Lobby Browser").Find("Viewport").Find("Content").Find("No Lobbies Text").gameObject.SetActive(true);
            }
        }
        else
        {
            transform.Find("Lobby Browser").Find("Viewport").Find("Content").Find("No Lobbies Text").gameObject.SetActive(true);
        }
    }
}

[System.Serializable]
class Server
{
    public List<string> ids;
    public List<string> names;
}

[System.Serializable]
class InitialJoinData
{
    public string player_id;
    public string server_id;
    public string server_code;
    public string nick;
}

[System.Serializable]
class HostData
{
    public string server_id;
}