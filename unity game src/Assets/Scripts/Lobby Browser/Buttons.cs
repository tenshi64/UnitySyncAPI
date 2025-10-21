using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    public int ServerID;
    private Button Button;

    void Start()
    {
        Button = GetComponent<Button>();

        if(gameObject.name.Contains("Join Server Button"))
        {
            Button.onClick.AddListener(() =>
            {
                transform.parent.parent.parent.parent.Find("Lobby Code").gameObject.SetActive(true);
                transform.parent.parent.parent.parent.Find("Lobby Code").Find("Lobby Name").GetComponent<TextMeshProUGUI>().text = transform.Find("Server Name").GetComponent<TextMeshProUGUI>().text;
                transform.parent.parent.parent.parent.Find("Lobby Code").Find("Join Button").GetComponent<Buttons>().ServerID = ServerID;
                transform.parent.parent.parent.gameObject.SetActive(false);
            });
        }
        else if (gameObject.name == "Join Button")
        {
            Button.onClick.AddListener(() =>
            {
                string _lobbyCode = Regex.Replace(transform.parent.GetComponent<TMP_InputField>().text, @"\s+", "");
                string _nick = Regex.Replace(transform.parent.Find("Nickname").GetComponent<TMP_InputField>().text, @"\s+", "");
                if (_lobbyCode.Length > 0)
                {
                    if(_nick.Length > 0)
                    {
                        string newUri = URI.JoinLobby.Replace("[SERVER-CODE]", _lobbyCode);
                        newUri = newUri.Replace("[SERVER-ID]", ServerID.ToString());
                        newUri = newUri.Replace("[NICKNAME]", _nick);
                        transform.parent.Find("Error Message").gameObject.SetActive(false);
                        transform.parent.parent.GetComponent<LobbyManager>().SendRequest(newUri);
                    }
                    else
                    {
                        transform.parent.Find("Error Message").GetComponent<TextMeshProUGUI>().text = "To join, you must enter your nickname.";
                        transform.parent.Find("Error Message").gameObject.SetActive(true);
                    }
                }
                else
                {
                    transform.parent.Find("Error Message").GetComponent<TextMeshProUGUI>().text = "To join, you must enter the code.";
                    transform.parent.Find("Error Message").gameObject.SetActive(true);
                }
            });
        }
        else if (gameObject.name == "Back Button")
        {
            Button.onClick.AddListener(() =>
            {
                transform.parent.parent.Find("Lobby Browser").gameObject.SetActive(true);
                transform.parent.gameObject.SetActive(false);
            });
        }
        else if (gameObject.name == "Refresh Button")
        {
            Button.onClick.AddListener(() =>
            {
                transform.parent.parent.GetComponent<LobbyManager>().SendRequest(URI.LobbyNames);
            });
        }
        else if (gameObject.name == "Host Button")
        {
            Button.onClick.AddListener(() =>
            {
                string _lobbyName = Regex.Replace(transform.parent.Find("Lobby Name").GetComponent<TMP_InputField>().text, @"\s+", "");
                string _nick = Regex.Replace(transform.parent.Find("Nickname").GetComponent<TMP_InputField>().text, @"\s+", "");
                if (_lobbyName.Length > 0)
                {
                    if (_nick.Length > 0)
                    {
                        transform.parent.parent.GetComponent<LobbyManager>().SendRequest(URI.GenerateLobbyCode);
                        transform.parent.Find("Error Message").gameObject.SetActive(false);
                    }
                    else
                    {
                        transform.parent.Find("Error Message").GetComponent<TextMeshProUGUI>().text = "To join, you must enter your nickname.";
                        transform.parent.Find("Error Message").gameObject.SetActive(true);
                    }
                }
                else
                {
                    transform.parent.Find("Error Message").GetComponent<TextMeshProUGUI>().text = "Lobby name must contain at least 1 character.";
                    transform.parent.Find("Error Message").gameObject.SetActive(true);
                }
            });
        }
    }
}
