<?php 
include_once "../classes/vector3.class.php";

abstract class Database
{
    private static $Host = "p:127.0.0.1";
    private static $Username = "root";
    private static $Password = "";
    private static $DatabaseName = "GameDB";
    private static $CodeLength = 6;

    private static $TimeBeforeKick = 30; //in seconds
    private static $TimeBeforeServerClosed = 30; //in seconds

    private static $DatabaseInstance;

    public static function EscapeString($_string) : string
    {
        return htmlspecialchars($_string);
    }

    public static function CreateConnection() : void
    {
        if(!isset(self::$DatabaseInstance))
        {
            $connection = new mysqli(self::$Host, self::$Username, self::$Password, self::$DatabaseName);

            if(mysqli_errno($connection))
            {
                echo "Failed to connect to the database: " . mysqli_connect_error();
                exit();
            }
            else
            {
                self::$DatabaseInstance = $connection;
            }
        }
    }

    public function __destruct()
    {
        self::CloseConnection();
    }

    public static function CloseConnection() : void
    {
        self::$DatabaseInstance->close();
    }

    //Server data

    public static function GetServers() : ?array
    {
        if(isset(self::$DatabaseInstance))
        {
            $_prompt = self::$DatabaseInstance->prepare("SELECT id, name FROM servers");
            $_prompt->execute();

            $_result = $_prompt->get_result();

            if($_result)
            {
                if($_result->num_rows > 0)
                {
                    $_nameArray = array();
                    $_idArray = array();
                    while($_row = $_result->fetch_row())
                    {
                        array_push($_nameArray, $_row[1]);
                        array_push($_idArray, $_row[0]);
                    }
                    
                    return array("ids" => $_idArray, "names" => $_nameArray);
                }
            }
        }
        
        return null;
    }

    public static function HostServer($_serverName, $_serverCode) : ?int
    {
        if(isset(self::$DatabaseInstance))
        {
            self::$DatabaseInstance->begin_transaction();

            try
            {
                $_time = time();

                $_prompt = self::$DatabaseInstance->prepare("INSERT INTO servers VALUES(null, ?, ?, ?)");
                $_prompt->bind_param("sss", $_serverName, $_serverCode, $_time);
                $_prompt->execute();

                self::$DatabaseInstance->commit();

                if($_prompt->affected_rows > 0)
                {                   
                    return $_prompt->insert_id;
                }
            }
            catch(mysqli_sql_exception $e)
            {
                self::$DatabaseInstance->rollback();
                throw new Exception($e->getMessage());
            }
        }
        
        return null;
    }

    public static function CompareServerCode($_serverID, $_serverCode) : bool
    {
        if(isset(self::$DatabaseInstance))
        {      
            $_prompt = self::$DatabaseInstance->prepare("SELECT code FROM servers WHERE id = ?");
            $_prompt->bind_param("i", $_serverID);
            $_prompt->execute();

            $_result = $_prompt->get_result();

            if($_result)
            {
                if($_result->num_rows > 0)
                {          
                    return ($_serverCode == $_result->fetch_row()[0]);
                }
            }
        }
        
        return false;
    }

    public static function RequestNewID($_nick) : ?int
    {
        if(isset(self::$DatabaseInstance))
        {     
            self::$DatabaseInstance->begin_transaction();

            try
            {
                $_time = time();

                $_prompt = self::$DatabaseInstance->prepare("INSERT INTO player_info VALUES(null, null, null, null, null, null, null, null, ?, ?)");
                $_prompt->bind_param("ss", $_time, $_nick);
                $_prompt->execute();

                self::$DatabaseInstance->commit();

                if($_prompt->affected_rows > 0)
                {                  
                    return $_prompt->insert_id;
                }
            }
            catch(mysqli_sql_exception $e)
            {
                self::$DatabaseInstance->rollback();
                throw new Exception($e->getMessage());
            }
        }
        
        return null;
    }

    public static function JoinServer($_playerID, $_serverID) : bool
    {
        if(isset(self::$DatabaseInstance))
        {     
            self::$DatabaseInstance->begin_transaction();

            try
            {
                $_time = time();

                $_prompt = self::$DatabaseInstance->prepare("UPDATE player_info SET server_id = ?, last_heartbeat = ? WHERE player_id = ?");
                $_prompt->bind_param("isi", $_serverID, $_time, $_playerID);
                $_prompt->execute();

                self::$DatabaseInstance->commit();

                if($_prompt->affected_rows > 0)
                {             
                    return true;
                }
            }
            catch(mysqli_sql_exception $e)
            {
                self::$DatabaseInstance->rollback();
                throw new Exception($e->getMessage());
            }
        }
        
        return false;
    }

    public static function GenerateLobbyCode() : ?string
    {
        $_characters = "abcdefghijklmnopqrstuvwxyz1234567890";
        $_code = "";

        for($i = 0; $i < self::$CodeLength; $i++)
        {
            $_code .= $_characters[rand(0, strlen($_characters)-1)];
        }

        $_prompt = self::$DatabaseInstance->prepare("SELECT id FROM servers WHERE code = ?");
        $_prompt->bind_param("s", $_code);
        $_prompt->execute();

        $_result = $_prompt->get_result();

        if($_result)
        {
            if($_result->num_rows == 0)
            {             
                return $_code;
            }
        }
        
        return "";
    }

    public static function KickPlayer($_playerID) : bool
    {
        if(isset(self::$DatabaseInstance))
        {     
            $_prompt = self::$DatabaseInstance->prepare("DELETE FROM player_info WHERE player_id = ?");
            $_prompt->bind_param("i", $_playerID);
            $_prompt->execute();

            if($_prompt->affected_rows > 0)
            {             
                return true;
            }
        }
        return false;
    }

    public static function CloseServer($_serverID) : void
    {
        if(isset(self::$DatabaseInstance))
        {      
            $_prompt = self::$DatabaseInstance->prepare("DELETE FROM server WHERE id = ?");
            $_prompt->bind_param("i", $_serverID);
            $_prompt->execute(); 
        }
    }

    public static function SendHeartbeat($_playerID, $_serverID) : void
    {
        if(isset(self::$DatabaseInstance))
        {      
            $_time = time();

            $_prompt = self::$DatabaseInstance->prepare("UPDATE player_info SET last_heartbeat = ? WHERE player_id = ? AND server_id = ?");
            $_prompt->bind_param("sii", $_time, $_playerID, $_serverID);
            $_prompt->execute(); 
        }
    }

    //Player data

    public static function UpdatePlayerPosition($_playerID, $_serverID, $_positionVector) : bool
    {
        if(isset(self::$DatabaseInstance))
        {
            $_time = time();
            $_positionVector = Vector3::EncodeVector3($_positionVector);

            $_prompt = self::$DatabaseInstance->prepare("UPDATE player_info SET posX = ?, posY = ?, posZ = ?, last_heartbeat = ? WHERE player_id = ? AND server_id = ?");
            $_prompt->bind_param("dddsii", $_positionVector::$X, $_positionVector::$Y, $_positionVector::$Z, $_time, $_playerID, $_serverID);
            $_prompt->execute();

            if($_prompt->affected_rows > 0)
            { 
                return true;
            }
        }
        
        return false;
    }

    public static function UpdatePlayerRotation($_playerID, $_serverID, $_rotationVector) : bool
    {
        if(isset(self::$DatabaseInstance))
        {
            $_time = time();
            $_rotationVector = Vector3::EncodeVector3($_rotationVector);

            $_prompt = self::$DatabaseInstance->prepare("UPDATE player_info SET rotX = ?, rotY = ?, rotZ = ?, last_heartbeat = ? WHERE player_id = ? AND server_id = ?");
            $_prompt->bind_param("dddsii", $_rotationVector::$X, $_rotationVector::$Y, $_rotationVector::$Z, $_time, $_playerID, $_serverID);
            $_prompt->execute();

            if($_prompt->affected_rows > 0)
            {
                return true;
            }
        }
        
        return false;
    }

    public static function UpdateEverything($_playerID, $_serverID, $_positionVector, $_rotationVector) : void
    {
        if(isset(self::$DatabaseInstance))
        {
            $_time = time();
            $_rotationVector = Vector3::EncodeVector3($_rotationVector);
            $_positionVector = Vector3::EncodeVector3($_positionVector);

            $_prompt = self::$DatabaseInstance->prepare("UPDATE player_info SET posX = ?, posY = ?, posZ = ?, rotX = ?, rotY = ?, rotZ = ?, last_heartbeat = ? WHERE player_id = ? AND server_id = ?");
            $_prompt->bind_param("ddddddsiii", $_positionVector::$X, $_positionVector::$Y, $_positionVector::$Z, $_rotationVector::$X, $_rotationVector::$Y, $_rotationVector::$Z, $_time, $_playerID, $_serverID);
            $_prompt->execute();
        }
    }

    //Get data

    public static function CheckIfServersShouldExist() : void
    {
        if(isset(self::$DatabaseInstance))
        {
            $_time = time();

            $_prompt = self::$DatabaseInstance->prepare("DELETE FROM servers WHERE servers.id NOT IN (SELECT server_id FROM player_info) AND ? - creation_time >= ?");
            $_prompt->bind_param("sd", $_time, self::$TimeBeforeServerClosed);
            $_prompt->execute();

            if($_prompt->affected_rows > 0)
            {
                echo "<br>Servers deleted...";
            }    
        }
    }

    public static function GetHeartbeats() : ?array
    {
        if(isset(self::$DatabaseInstance))
        {
            $_time = time();

            $_prompt = self::$DatabaseInstance->prepare("SELECT player_id FROM player_info WHERE ? - last_heartbeat >= ? OR last_heartbeat IS NULL");
            $_prompt->bind_param("sd", $_time, self::$TimeBeforeKick);
            $_prompt->execute();

            $_result = $_prompt->get_result();

            if($_result)
            {
                if($_result->num_rows > 0)
                {
                    $_ids = array();
                    while($_row = $_result->fetch_row())
                    {
                        array_push($_ids, $_row[0]);
                    }
                    
                    return $_ids;
                }
            }
        }
        
        return null;
    }

    public static function GetPlayerPosition($_playerID, $_serverID) : ?Vector3
    {
        if(isset(self::$DatabaseInstance))
        {
            $_prompt = self::$DatabaseInstance->prepare("SELECT posX, posY, posZ FROM player_info WHERE player_id = ? AND server_id = ?");
            $_prompt->bind_param("ii", $_playerID, $_serverID);
            $_prompt->execute();

            $_result = $_prompt->get_result();

            if($_result)
            {
                if($_result->num_rows > 0)
                {
                    $_position = $_result->fetch_row();
              
                    return new Vector3($_position[0], $_position[1], $_position[2]);
                }
            }
        }
        
        return null;
    }

    public static function GetPlayerRotation($_playerID, $_serverID) : ?Vector3
    {
        if(isset(self::$DatabaseInstance))
        {
            $_prompt = self::$DatabaseInstance->prepare("SELECT rotX, rotY, rotZ FROM player_info WHERE player_id = ? AND server_id = ?");
            $_prompt->bind_param("ii", $_playerID, $_serverID);
            $_prompt->execute();

            $_result = $_prompt->get_result();

            if($_result)
            {
                if($_result->num_rows > 0)
                {
                    $_rotation = $_result->fetch_row();
            
                    return new Vector3($_rotation[0], $_rotation[1], $_rotation[2]);
                }
            }
        }
        
        return null;
    }

    public static function GetAllPlayerIDs($_serverID) : ?array
    {
        if(isset(self::$DatabaseInstance))
        {
            $_prompt = self::$DatabaseInstance->prepare("SELECT player_id FROM player_info WHERE server_id = ?");
            $_prompt->bind_param("i", $_serverID);
            $_prompt->execute();

            $_result = $_prompt->get_result();

            if($_result)
            {
                if($_result->num_rows > 0)
                {
                    $_idArray = array();
                    while($_row = $_result->fetch_row())
                    {
                        array_push($_idArray, $_row);
                    }
                    
                    return $_idArray;
                }
            }
        }
        
        return null;
    }

    public static function GetAllInfoExcept($_playerID, $_serverID) : ?array
    {
        if(isset(self::$DatabaseInstance))
        {
            $_prompt = self::$DatabaseInstance->prepare("SELECT player_id, posX, posY, posZ, rotX, rotY, rotZ, nick, last_heartbeat FROM player_info WHERE server_id = ? AND player_id != ?");
            $_prompt->bind_param("ii", $_serverID, $_playerID);
            $_prompt->execute();

            $_result = $_prompt->get_result();

            if($_result)
            {
                if($_result->num_rows > 0)
                {
                    $_playerIDs = array();
                    $_positions = array();
                    $_rotations = array();
                    $_nicks = array();
                    $_heartbeats = array();
                    while($_row = $_result->fetch_row())
                    {
                        array_push($_playerIDs, $_row[0]);
                        array_push($_positions, "(" . $_row[1] . "|" . $_row[2] . "|" . $_row[3] . ")");
                        array_push($_rotations, "(" . $_row[4] . "|" . $_row[5] . "|" . $_row[6] . ")");
                        array_push($_nicks, $_row[7]);
                        array_push($_heartbeats, $_row[8]);
                    }
                    
                    return array("ids" => $_playerIDs, "positions" => $_positions, "rotations" => $_rotations, "nicknames" => $_nicks, "heartbeats" => $_heartbeats);
                }
            }
        }   
        return null;
    }
}

?>