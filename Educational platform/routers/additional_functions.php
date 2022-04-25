<?php
    function login_after_registration($connection, $urlList, $requestData)
    {
        if (!$connection){
            echo 'error in Additional functions'. PHP_EOL;
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

    function updateRole($connection, $user_id, $role){
        if (!$connection){
            return false;
            echo 'error in Additional functions #2'. PHP_EOL;
        }

        if ($role == '1' || $role == '2' || $role == '3')
        {
            
            $findUser = $connection->query("SELECT user_id FROM tokens WHERE user_id='$user_id' ")->fetch_assoc();
            $findUser = $findUser["user_id"];
            
            if (empty($findUser)){
                HTTPStatus("404", "No such user to update", "No such user to update");
            }
            else{
                $roleInsertResult = $connection->query(" UPDATE users SET role_id = '$role' WHERE user_id = '$user_id' ");

                if (!$roleInsertResult){
                    HTTPStatus("400", "Bad request", "cannot add new role in additional functions");
                    return false;
                }
                else{
                    echo "role sucessfully added!";
                    return true;
                }
            }
        }
        else{
            HTTPStatus("400", "Given role is out of range", "Given role is out of range");
            return false;
        }
 
    }

    function HTTPStatus($code, $text, $information) {
        $protocol = (isset($_SERVER['SERVER_PROTOCOL']) ? $_SERVER['SERVER_PROTOCOL'] : 'HTTP/1.0');
        header($protocol . ' ' . $code . ' ' . $text);
        $GLOBALS['http_response_code'] = $code;
        $message =
            [
                "status" => "false",
                "Error" => $information,
            ];
        echo json_encode($message);
    }

    function checkValidToken($connection, $token){
        if (empty($token)){
            echo "You got no token" . PHP_EOL;
            return false;
        }
        else{
            $findUser = $connection->query("SELECT user_id FROM tokens WHERE value='$token' ")->fetch_assoc();
                    $findUser = $findUser["user_id"];
                    if (empty($findUser)){
                        echo "Your token is not valid" . PHP_EOL;
                        return false;
                    }
                    else{
                        return true;
                    }
        }
    }

    function checkIfAdmin($connection, $token){
        $findUser = $connection->query("SELECT user_id FROM tokens WHERE value='$token' ")->fetch_assoc();
        $findUser = $findUser["user_id"];
        if (empty($findUser)){
            //echo "Your token is not valid" . PHP_EOL;
            return false;
        }
        else{
            $findRole = $connection->query("SELECT role_id FROM users WHERE user_id='$findUser' ")->fetch_assoc();
            if ($findRole["role_id"] == "2" || $findRole["role_id"] == "3"){
                //echo "you are admin";
                return true;
            }
            else{
                //echo "You are not admin, :";
                //echo json_encode($findRole["name"]) . PHP_EOL;
                return false;
            }   
        }
    }

    function checkIfOwner($connection, $token, $given_id){
        $findUser = $connection->query("SELECT user_id FROM tokens WHERE value='$token' ")->fetch_assoc();
        $findUser = $findUser["user_id"];
        if (empty($findUser)){
            //echo "Your token is not valid" . PHP_EOL;
            return false;
        }
        else{
            if ($findUser == $given_id){
                //echo "you are owner";
                return true;
            }
            else{
                //echo "You are not owner, :";
                return false;
            }   
        }
    }
?>