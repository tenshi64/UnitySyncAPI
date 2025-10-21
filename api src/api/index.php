<?php
    include_once("../classes/filename.class.php");
    Filename::Initialize();

    include_once("../classes/database.class.php");
    include_once("../classes/vector3.class.php");

    Database::CreateConnection();

    $_operationType = isset($_GET["type"]) ? Database::EscapeString($_GET["type"]) : null;
    $_mode = isset($_GET["mode"]) ? Database::EscapeString($_GET["mode"]) : null;
    $_serverID = isset($_GET["server-id"]) ? Database::EscapeString($_GET["server-id"]) : null;
    $_serverName = isset($_GET["server-name"]) ? Database::EscapeString($_GET["server-name"]) : null;
    $_serverCode = isset($_GET["server-code"]) ? Database::EscapeString($_GET["server-code"]) : null;
    $_playerID = isset($_GET["player-id"]) ? Database::EscapeString($_GET["player-id"]) : null;
    $_positionVector = isset($_GET["position"]) ? Database::EscapeString($_GET["position"]) : null;
    $_rotationVector = isset($_GET["rotation"]) ? Database::EscapeString($_GET["rotation"]) : null;
    $_nick = isset($_GET["nick"]) ? Database::EscapeString($_GET["nick"]) : null;

    if(Filename::VariableExists($_operationType))
    {
        if(strtolower($_operationType) == "server")
        {
            if(Filename::VariableExists($_mode))
            {
                if(strtolower($_mode) == "join-server")
                {
                    if(Filename::VariableExists($_serverID) && Filename::VariableExists($_serverCode) && Filename::VariableExists($_nick))
                    {
                        try
                        {
                            $_codeValidation = Database::CompareServerCode($_serverID, $_serverCode);
                            if(!$_codeValidation)
                            {
                                echo "-1";
                            }
                            else
                            {
                                $_id = Database::RequestNewID($_nick);
                                $_response = Database::JoinServer($_id, $_serverID);

                                if($_response)
                                {
                                    echo json_encode(array("player_id" => $_id, "server_id" => $_serverID, "server_code" => $_serverCode, "nick" => $_nick));
                                }
                                else
                                {
                                    echo "failed";
                                }
                            }
                        }
                        catch(Exception $e)
                        {
                            throw new Exception($e->getMessage());
                        }
                    }
                }
                else if(strtolower($_mode) == "host-server")
                {
                    if(Filename::VariableExists($_serverName) && Filename::VariableExists($_serverCode))
                    {
                        try
                        {
                            $_response = Database::HostServer($_serverName, $_serverCode);
                            if($_response != null)
                            {
                                echo json_encode(array("server_id" => $_response));
                            }
                            else
                            {
                                echo "failed";
                            }
                        }
                        catch(Exception $e)
                        {
                            throw new Exception($e->getMessage());
                        }
                    }
                }
                else if(strtolower($_mode) == "heartbeat")
                {
                    if(Filename::VariableExists($_playerID) && Filename::VariableExists($_serverID))
                    {
                        try
                        {
                            Database::SendHeartbeat($_playerID, $_serverID);
                            echo "ok";
                        }
                        catch(Exception $e)
                        {
                            throw new Exception($e->getMessage());
                        }
                    }
                }
                else if(strtolower($_mode) == "get-code")
                {
                    try
                    {
                        $_response = Database::GenerateLobbyCode();
                        while(!$_response)
                        {
                            $_response = Database::GenerateLobbyCode();
                        }
                        echo $_response;
                    }
                    catch(Exception $e)
                    {
                        throw new Exception($e->getMessage());
                    }
                }
                else if(strtolower($_mode) == "kick-player")
                {
                    if(Filename::VariableExists($_playerID))
                    {
                        try
                        {
                            echo Database::KickPlayer($_playerID) ? "ok" : "failed";
                        }
                        catch(Exception $e)
                        {
                            throw new Exception($e->getMessage());
                        }
                    }
                }
            }
        }
        else if(strtolower($_operationType) == "update")
        {
            if(Filename::VariableExists($_mode))
            {
                if(strtolower($_mode) == "position")
                {
                    if(Filename::VariableExists($_serverID) && Filename::VariableExists($_playerID) && Filename::VariableExists($_positionVector))
                    {
                        try
                        {
                            echo Database::UpdatePlayerPosition($_playerID, $_serverID, $_positionVector) ? "ok" : "failed";
                        }
                        catch(Exception $e)
                        {
                            throw new Exception($e->getMessage());
                        }
                    }
                }
                else if(strtolower($_mode) == "rotation")
                {
                    if(Filename::VariableExists($_serverID) && Filename::VariableExists($_playerID) && Filename::VariableExists($_rotationVector))
                    {
                        try
                        {
                            echo Database::UpdatePlayerRotation($_playerID, $_serverID, $_rotationVector) ? "ok" : "failed";
                        }
                        catch(Exception $e)
                        {
                            throw new Exception($e->getMessage());
                        }
                    }
                }
            }
        }
        else if(strtolower($_operationType) == "get-info")
        {
            if(Filename::VariableExists($_mode))
            {
                if(strtolower($_mode) == "lobby-list")
                {
                    try
                    {
                        $_array = Database::GetServers();
                        $_json = json_encode($_array);
                        
                        echo $_json;
                    }
                    catch(Exception $e)
                    {
                        throw new Exception($e->getMessage());
                    }
                }
                else if(strtolower($_mode) == "position")
                {
                    if(Filename::VariableExists($_serverID) && Filename::VariableExists($_playerID))
                    {
                        try
                        {
                            Database::GetPlayerPosition($_playerID, $_serverID);
                        }
                        catch(Exception $e)
                        {
                            throw new Exception($e->getMessage());
                        }
                    }
                }
                else if(strtolower($_mode) == "rotation")
                {
                    if(Filename::VariableExists($_serverID) && Filename::VariableExists($_playerID))
                    {
                        try
                        {
                            Database::GetPlayerRotation($_playerID, $_serverID);
                        }
                        catch(Exception $e)
                        {
                            throw new Exception($e->getMessage());
                        }
                    }
                }
                else if(strtolower($_mode) == "receive-data")
                {
                    if(Filename::VariableExists($_serverID) && Filename::VariableExists($_playerID))
                    {
                        try
                        {
                            //Database::UpdateEverything($_playerID, $_serverID, Vector3::EncodeVector3($_positionVector), Vector3::EncodeVector3($_rotationVector));
                            echo json_encode(Database::GetAllInfoExcept($_playerID, $_serverID));
                        }
                        catch(Exception $e)
                        {
                            throw new Exception($e->getMessage());
                        }
                    }
                }
            }
        }
    }

    Database::CloseConnection();
?>