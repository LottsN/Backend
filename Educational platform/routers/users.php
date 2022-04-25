<?php
    include_once "additional_functions.php";
    
    function route($connection, $urlList, $requestData){
        $token = substr(getallheaders()['Authorization'], 7);
        if (!empty($urlList)){
            $user_id = $urlList[0];
            if (is_numeric($user_id)){
                if ($urlList[1] == "role"){
                    //update user's role
                    if ($requestData->method != "POST"){
                        //доделать код ошибки
                        HTTPStatus("400", "Bad request", "Not POST method");
                    }else{
                        if (!empty( $token )){
                            if (checkIfAdmin($connection, $token)){
                                $requestBody = $requestData->body;
                                $given_role = $requestBody->role_id;
                                updateRole($connection, $user_id, $given_role);
                            }
                            else{
                                HTTPStatus("403", "You do not have permission to view information about this user", "You do not have permission to view information about this user");
                            }

                        }
                        else{
                            HTTPStatus("403", "You must be logged in", "Your got no token");
                        }
                    }
                }
                else{
                    if (empty($urlList[1])){
                        $method = $requestData->method;
                        switch($method){
                            case 'GET':
                                if (!empty( $token )){
                                    if (checkIfAdmin($connection, $token) || checkIfOwner($connection, $token, $user_id)){
                                        $givenUserById = $connection->query("SELECT user_id, username, role_id, name, surname FROM users WHERE user_id='$user_id'")->fetch_assoc();
                                        if (!is_null($givenUserById)){
                                            echo json_encode($givenUserById);
                                        }
                                        else {
                                            HTTPStatus("400", "No user with this ID", "No user with this ID");
                                        }
                                    }
                                    else{
                                        HTTPStatus("403", "You do not have permission to view information about this user",  "You do not have permission to view information about this user");
                                    }

                                }
                                else {
                                    HTTPStatus("403", "You got no token", "You got no token");
                                }
                                break;
                            case 'DELETE':
                                if (!empty( $token )){
                                    if (checkIfAdmin($connection, $token)){
                                        $givenUserById = $connection->query("SELECT user_id FROM users WHERE user_id='$user_id'")->fetch_assoc();
                                        if (!empty($givenUserById)){
                                            $deleteResult = $connection->query("DELETE FROM users WHERE user_id='$user_id'");
                                            if ($deleteResult){
                                                $logoutResult = $connection->query(" DELETE FROM tokens WHERE user_id='$user_id'  ");
                                                if ($logoutResult){
                                                    echo "success delete";
                                                }else{
                                                    HTTPStatus("400", "Cannot logout user", "Cannot logout user");
                                                }
                                            }
                                            else {
                                                HTTPStatus("500", "Unexpected Error :(", "Unexpected Error :(");
                                            }
                                        }else{
                                            HTTPStatus("404", "User is not exist", "User is not exist");
                                        }
                                    }
                                    else{
                                        HTTPStatus("403", "You do not have permission to delete this user",  "You do not have permission to delete this user");
                                    }
                                }
                                else {
                                     HTTPStatus("403", "You got no token", "You got no token");
                                }
                                break;
                                case 'PATCH':
                                    if (!empty( $token )){
                                        if (checkIfAdmin($connection, $token) || checkIfOwner($connection, $token, $user_id) ){
                                            $requestBody = $requestData->body;
                                            $requestPass = hash('sha1', $requestBody->password);
                                            $requestName = $requestBody->name;
                                            $requestSurname = $requestBody->surname; 

                                            $givenUserByToken = $connection->query("SELECT user_id FROM tokens WHERE value='$token' ")->fetch_assoc();
                                            $givenUserByToken = $givenUserByToken["user_id"];

                                            if (!empty($givenUserByToken)){
                                                $patchResult = $connection->query(" UPDATE users SET password = '$requestPass', name = '$requestName', surname = '$requestSurname'  WHERE user_id='$user_id' ");
                                                if ($patchResult){
                                                    $givenUserById = $connection->query("SELECT password, name, surname FROM users WHERE user_id='$user_id'")->fetch_assoc();
                                                    if (!is_null($givenUserById)){
                                                        echo json_encode($givenUserById);
                                                    }
                                                    else {
                                                        HTTPStatus("500", "Unexpected Error :(", "Unexpected Error :(");
                                                    }
                                                }
                                                else {
                                                    HTTPStatus("500", "Unexpected Error :(", "Unexpected Error :(");
                                                }
                                            }else{
                                                HTTPStatus("404", "User is not exist", "User is not exist");
                                            }
                                        }
                                        else{
                                            HTTPStatus("403", "You do not have permission to patch this user",  "You do not have permission to patch this user");
                                        }
                                    }
                                    else {
                                         HTTPStatus("403", "You got no token", "You got no token");
                                    }
                                    break;
        
                            default:
                                HTTPStatus("400", "Method POST is not allowed", "Method POST is not allowed");
                                break; 
                        }
                    }
                    else{
                        HTTPStatus("400", "Bad request", "No method '$urlList[1]' ");
                    }
                }
            }
            else {
                HTTPStatus("400", "User ID must be a number", "User ID must be a number");
            }
        }
        else {
            if ($requestData->method == 'GET'){
                if (checkValidToken($connection, $token)){

                    if (checkIfAdmin($connection, $token)){
                        $users = $connection->query("SELECT user_id, username, role_id from users");
                        if (!is_null($users)){
                            $usersArr = [];
                            foreach ($users as $user_id){
                                $usersArr[] = $user_id;
                            }    
                            echo json_encode($usersArr);
                        }
                        else {
                            HTTPStatus("500", "Something went wrong");
                        }
                    }
                    else {
                        HTTPStatus("403", "You are not an administrator", "You are not admin");
                    }
                }

                else{
                    HTTPStatus("403", "You must be logged in", "Your token is not valid");
                }
            }
            else {
                HTTPStatus("400", "Bad request", "Not GET method");
            }
        }
    }
          

    

?>