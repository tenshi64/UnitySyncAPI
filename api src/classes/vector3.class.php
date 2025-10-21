<?php

class Vector3
{
    public static $X;
    public static $Y;
    public static $Z;

    public function __construct($_x, $_y, $_z)
    {
        if(is_float($_x) && is_float($_y) && is_float($_z))
        {
            self::$X = $_x;
            self::$Y = $_y;
            self::$Z = $_z;
        }
        else
        {
            throw new Exception("Values must be of type float.");
        }
    }

    public static function DecodeVector3(Vector3 $_vector3) : string
    {
        return "(" . $_vector3::$X . "|" .$_vector3::$Y . "|" . $_vector3::$Z . ")";
    }

    public static function EncodeVector3(string $_stringVector3) : ?Vector3
    {
        $_firstBracket = -1;
        $_lastBracket = -1;
        $_firstSeparator = -1;
        $_lastSeparator = -1;
        for($i = 0; $i < strlen($_stringVector3); $i++)
        {
            if($_stringVector3[$i] == "(")
            {
                $_firstBracket = $i;
            }
            else if($_stringVector3[$i] == ")")
            {
                $_lastBracket = $i;
            }
            else if($_stringVector3[$i] == "|")
            {
                if($_firstSeparator == -1)
                {
                    $_firstSeparator = $i;
                }
                else
                {
                    $_lastSeparator = $i;
                }
            }
        }

        $_firstNumber = substr($_stringVector3, $_firstBracket+1, $_firstSeparator - $_firstBracket - 1);
        $_secondNumber = substr($_stringVector3, $_firstSeparator+1, $_lastSeparator - $_firstSeparator - 1);
        $_thirdNumber = substr($_stringVector3, $_lastSeparator+1, $_lastBracket - $_lastSeparator - 1);

        return new Vector3(floatval($_firstNumber), floatval($_secondNumber), floatval($_thirdNumber));
    }
}

?>