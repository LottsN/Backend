<?php
    function route($connection, $urlList, $requestData)
    {
        if (!$connection){
            echo 'error in Login'. PHP_EOL;
        }

        //echo json_encode($urlList) . PHP_EOL;
        //echo json_encode($requestData) . PHP_EOL;

        if ($requestData->method != "POST"){
            //доделать код ошибки
            echo 'not POST method';
        }
        else{
            $username = $requestData->body->username;
            $password = hash('sha1', $requestData->body->password);

            $user = $connection->query(" SELECT user_id FROM users WHERE username='$username' AND password='$password' ")->fetch_assoc();

            if (!is_null($user)){
                $ids = $user['user_id'];
                $sessions = $connection->query("SELECT 'value' FROM tokens WHERE user_id='$ids' ");

                if ($sessions->num_rows > 0){
                    echo 'User have already logined';
                }
                else{
                    $token = bin2hex(random_bytes(16));
                    $user_id = $user['user_id'];
                    $valid_until = date( "Y:m:d", strtotime("+30 days"));
                    //echo $valid_until;
                    $tokenInsertResult = $connection->query(" INSERT INTO tokens(value, user_id, valid_until) VALUES('$token', '$user_id', '$valid_until') ");
                    if (!$tokenInsertResult){
                        echo json_encode($connection->error);
                    }
                    else{
                        echo json_encode(['token' => $token]);
                    }
                }    
            }
            else{
                echo 'Unregistred user';
            }
        }
    }
?>