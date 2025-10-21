<?php

abstract class Filename
{
    public static function Initialize()
    {
        $_url = (empty($_SERVER['HTTPS']) ? 'http' : 'https') . "://$_SERVER[HTTP_HOST]$_SERVER[REQUEST_URI]";

        if(str_contains($_url, ".php"))
        {
            $_lastFolder = self::Get_LastFolder($_url);
            if($_lastFolder != "error")
            {
                header("location: .." . $_lastFolder);
            }
        }
    }

    private static function Get_LastFolder($_url) : string
    {
        $_url = strtolower($_url);
        $_extensionIndex = strpos($_url, ".php");

        $_firstSlash = -1;
        $_lastSlash = -1;

        for($i = $_extensionIndex; $i > 0; $i--)
        {
            if($_url[$i] == "\\" || $_url[$i] == "/")
            {
                if($_firstSlash == -1)
                {
                    $_firstSlash = $i;
                }
                else
                {
                    $_lastSlash = $i;
                    break;
                }
            }
        }

        if($_firstSlash != -1 && $_lastSlash != -1)
        {
            return substr($_url, $_lastSlash, (strlen($_url) - $_lastSlash) - (strlen($_url) - $_firstSlash));
        }
        return "error";
    }

    public static function VariableExists($_variable) : bool
    {
        if(strlen((string)$_variable) > 0)
        {
            return true;
        }

        return false;
    }
}

?>