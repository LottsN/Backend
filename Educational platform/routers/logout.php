<?php
    function route($connection, $urlList, $requestData)
    {
        if (!$connection){
            echo 'error in Logout'. PHP_EOL;
        }

        //echo json_encode($urlList) . PHP_EOL;
        //echo json_encode($requestData) . PHP_EOL;

        if ($requestData->method != "POST"){
            //доделать код ошибки
            echo 'not POST method';
        }
        else{
            $token = substr(getallheaders()['Authorization'], 7);
            if ($token == null){
                echo 'No token/ bad request';
            }
            else{
                echo json_encode($token);
                $sessions = $connection->query("SELECT * FROM tokens WHERE value='$token' ");
                if ($sessions->num_rows > 0){
                    $sessions = $connection->query("DELETE FROM tokens WHERE value='$token' ");
                    echo 'Session succesfully ended' . PHP_EOL;
                }
                else{
                    echo 'This session have already canceled';
                }
            }
        }


    }
?>