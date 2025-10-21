<link rel="stylesheet" href="../styles/default-style.css">
<script src="../scripts/interval.function.js"></script>
<?php
    include_once("../classes/filename.class.php");
    Filename::Initialize();

    include_once("../classes/database.class.php");
    Database::CreateConnection();

    $_array = Database::GetHeartbeats();
    echo json_encode($_array);

    if($_array != null)
    {
        if(count($_array) > 0)
        {
            for ($i = 0; $i < count($_array); $i++)
            {
                Database::KickPlayer($_array[$i]);
            }
        }
    }
    Database::CheckIfServersShouldExist();
?>