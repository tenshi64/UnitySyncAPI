using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class URI
{
    public const string LobbyNames = "http://127.0.0.1/Simple%20Game%20API/api/?type=get-info&mode=lobby-list";
    public const string RequestNewID = "http://127.0.0.1/Simple%20Game%20API/api/?type=update-player&mode=request-id";
    public const string HostLobby = "http://127.0.0.1/Simple%20Game%20API/api/?type=server&mode=host-server&server-name=[SERVER-NAME]&server-code=[SERVER-CODE]";
    public const string JoinLobby = "http://127.0.0.1/Simple%20Game%20API/api/?type=server&mode=join-server&server-code=[SERVER-CODE]&server-id=[SERVER-ID]&nick=[NICKNAME]";
    public const string GenerateLobbyCode = "http://127.0.0.1/Simple%20Game%20API/api/?type=server&mode=get-code";
    public const string SendHeartbeat = "http://127.0.0.1/Simple%20Game%20API/api/?type=server&mode=heartbeat&player-id=[PLAYER-ID]&server-id=[SERVER-ID]";
    public const string ReceiveData = "http://127.0.0.1/Simple%20Game%20API/api/?type=get-info&mode=receive-data&player-id=[PLAYER-ID]&server-id=[SERVER-ID]";
    public const string SendData = "http://127.0.0.1/Simple%20Game%20API/api/?type=update&mode=[MODE]&player-id=[PLAYER-ID]&server-id=[SERVER-ID]&[DATA-TYPE]=[DATA]";
    public const string KickPlayer = "http://127.0.0.1/Simple%20Game%20API/api/?type=server&mode=kick-player&player-id=[PLAYER-ID]";

    public static bool CompareURIs(string _uri, string _originalUri)
    {
        for (int i = 0; i < _uri.Length; i++)
        {
            if(_uri[i] != _originalUri[i])
            {
                if (_originalUri[i] == '[' || _originalUri[i] == ']')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }
}