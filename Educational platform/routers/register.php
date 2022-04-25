<?php
    function route($connection, $urlList, $requestData)
    {
        if (!$connection){
            echo 'error in connection'. PHP_EOL;
        }

        //echo json_encode($urlList) . PHP_EOL;
        //echo json_encode($requestData) . PHP_EOL;

        if ($requestData->method != "POST"){
            //доделать код ошибки
            echo 'Not POST method used';
        }
        else{
            $username = $requestData->body->username;

            $user = $connection->query("SELECT user_id FROM users WHERE username='$username'")->fetch_assoc();

            if (is_null($user)){
                $name =  $requestData->body->name;
                $surname = $requestData->body->surname;
                $password = hash('sha1', $requestData->body->password);

                $userInsertResult = $connection->query("INSERT INTO users(name, surname, username, password, role_id) VALUES ('$name', '$surname', '$username', '$password', '1') ");

                //echo($userInsertResult);

                if (!$userInsertResult){
                    
                    echo 'cannot insert new user' . PHP_EOL;
                }
                else{
                    //echo 'successfully inserted new user!'. PHP_EOL;
                    if (file_exists(realpath(dirname(__FILE__))."/additional_functions.php" ))
                        {
                            $getUser = $connection->query("SELECT user_id FROM users WHERE username='$username'")->fetch_assoc();
                            //echo ($getUser["user_id"]);
                            include_once "additional_functions.php";
                            login_after_registration($connection, $urlList, $requestData);
                        }
                    else
                        {
                            //echo "404, file not found";
                        }
                }
                /*
                echo $username . PHP_EOL;
                echo $name . PHP_EOL;
                echo $surname . PHP_EOL;
                echo $password . PHP_EOL;
                */
            }
            else{
                echo 'USER EXIST';
            }
        }
    }
?>