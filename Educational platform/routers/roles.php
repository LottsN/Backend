<?php
    include_once "additional_functions.php";

    function route($connection, $urlList, $requestData)
    {
        if (!$connection){
            echo 'error in roles'. PHP_EOL;
        }

        //echo json_encode($urlList) . PHP_EOL;
        //echo json_encode($requestData) . PHP_EOL;

        if ($requestData->method != "GET"){
            //доделать код ошибки
            echo 'not GET method';
        }
        else{
            $token = substr(getallheaders()['Authorization'], 7);
            
            if ($token == null){
                echo 'No token/ bad request' . PHP_EOL;
            }
            else{
                if (empty($urlList)){
                    //echo json_encode($token) . PHP_EOL;
                    $findUser = $connection->query("SELECT user_id FROM tokens WHERE value='$token' ")->fetch_assoc();
                    $findUser = $findUser["user_id"];

                    //chach valid
                    if ($findUser == 0){
                        
                        echo "Your token is not valid";
                    }
                    else{
                        //echo ($findUser) . PHP_EOL;
                        $message = [];
                        $allRoles = $connection->query("SELECT role_id, name FROM roles ");
    
                        if (!$allRoles)
                        {
                            echo "Не удалось выполнить запрос: (" . $mysqli->errno . ") " . $mysqli->error;
                        }
                        else{
                            while ($row = $allRoles->fetch_assoc())
                            {
                                $message[] =
                                [
                                    "role_id" => $row["role_id"],
                                    "name" => $row["name"],
                                ];
                            }
                        }
                        echo json_encode($message);   
                    }

                }
                else{
                    if ( !is_numeric($urlList[0]) )
                    {
                        HTTPStatus("404", "No such role_id", "No such role_id");
                    }
                    else{
                        $role_id = $urlList[0];
                        $getRoleResult = $connection->query("SELECT role_id, name FROM roles where role_id = '$role_id' ")->fetch_assoc();

                        if (empty($getRoleResult)   )
                        {
                            HTTPStatus("404", "No such role_id", "No such role_id");
                        }
                        else{
                            $message =
                                [
                                    "roleId" => $getRoleResult["role_id"],
                                    "name" => $getRoleResult["name"],
                                ];
                                
                            echo json_encode($message);     
                        }
                    } 
                }       
            }
        }
    }
?>