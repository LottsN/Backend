<?php
    include_once "additional_functions.php";
    
    function route($connection, $urlList, $requestData){
        $token = substr(getallheaders()['Authorization'], 7);
        if (!empty($urlList)){
            $topic_id = $urlList[0];
            if (is_numeric($topic_id)){
                if ($urlList[1] == "childs"){
                    $method = $requestData->method;
                    switch($method){
                        case 'GET':
                            $givenTopicById = $connection->query("SELECT id, name, parent_id FROM topics WHERE id='$topic_id' ")->fetch_assoc();
                            if (!is_null($givenTopicById)){
                                $topicInfo = [];
                                $top_id = $givenTopicById["id"];
                                $child = $connection->query("SELECT id, name, parent_id FROM topics WHERE parent_id = '$top_id' ");
                                
                                while ($row = $child->fetch_assoc())
                                {
                                    $topicInfo[] =
                                    [
                                        "id" => $row["id"],
                                        "name" => $row["name"],
                                        "parent_id" => $row["parent_id"],
                                    ];  
                                }

                                echo json_encode($topicInfo);
                            }
                            else {
                                HTTPStatus("400", "No child of topic with this ID", "No child of topic with this ID");
                            }
                            break;

                        case 'PATCH':
                            if (!empty( $token )){
                                if (checkIfAdmin($connection, $token) ){
                                    $givenTopicById = $connection->query("SELECT id, name, parent_id FROM topics WHERE id='$topic_id' ")->fetch_assoc();
                                    if (!is_null($givenTopicById)){
                                        foreach ($requestData->body as $value){
                                            $patchResult = $connection->query(" UPDATE topics SET parent_id = '$topic_id' WHERE id ='$value' ");
                                            if (!$patchResult){ 
                                                HTTPStatus("500", "Unexpected Error :(", "Unexpected Error :(");
                                            }
                                        }


                                        $topicInfo = [];
                                        $topicInfo["id"] = $givenTopicById["id"];
                                        $topicInfo["name"] = $givenTopicById["name"];
                                        $topicInfo["parent_id"] = $givenTopicById["parent_id"];
                                        $top_id = $givenTopicById["id"];
                                        $child = $connection->query("SELECT id, name, parent_id FROM topics WHERE parent_id = '$top_id' ");
                                        
                                        while ($row = $child->fetch_assoc())
                                        {
                                            $topicInfo["childs"][] =
                                            [
                                                "id" => $row["id"],
                                                "name" => $row["name"],
                                                "parent_id" => $row["parent_id"],
                                            ];  
                                        }

                                        echo json_encode($topicInfo);


                                    }
                                    else {
                                        HTTPStatus("400", "No topic with this ID", "No topic with this ID");
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

                        case 'DELETE':
                            if (!empty( $token )){
                                if (checkIfAdmin($connection, $token) ){
                                    $givenTopicById = $connection->query("SELECT id, name, parent_id FROM topics WHERE id='$topic_id' ")->fetch_assoc();
                                    if (!is_null($givenTopicById)){
                                        foreach ($requestData->body as $value){
                                            $deleteResult = $connection->query(" DELETE FROM topics WHERE id ='$value' ");
                                            if (!$deleteResult){ 
                                                HTTPStatus("500", "Unexpected Error :(", "Unexpected Error :(");
                                            }
                                        }

                                        $topicInfo = [];
                                        $topicInfo["id"] = $givenTopicById["id"];
                                        $topicInfo["name"] = $givenTopicById["name"];
                                        $topicInfo["parent_id"] = $givenTopicById["parent_id"];
                                        $topicInfo["childs"] = [];
                                        $top_id = $givenTopicById["id"];
                                        $child = $connection->query("SELECT id, name, parent_id FROM topics WHERE parent_id = '$top_id' ");
                                        
                                        while ($row = $child->fetch_assoc())
                                        {
                                            $topicInfo["childs"][] =
                                            [
                                                "id" => $row["id"],
                                                "name" => $row["name"],
                                                "parent_id" => $row["parent_id"],
                                            ];  
                                        }
                                        echo json_encode($topicInfo);
                                    }
                                    else {
                                        HTTPStatus("400", "No topic with this ID", "No topic with this ID");
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
                    if (empty($urlList[1])){
                        $method = $requestData->method;
                        switch($method){
                            case 'GET':
                                    $givenTopicById = $connection->query("SELECT id, name, parent_id FROM topics WHERE id='$topic_id' ")->fetch_assoc();
                                    if (!is_null($givenTopicById)){
                                        $topicInfo = [];
                                        $topicInfo["id"] = $givenTopicById["id"];
                                        $topicInfo["name"] = $givenTopicById["name"];
                                        $topicInfo["parent_id"] = $givenTopicById["parent_id"];
                                        $top_id = $givenTopicById["id"];

                                        $child = $connection->query("SELECT id, name, parent_id FROM topics WHERE parent_id = '$top_id' ");
                                        if (!is_null($child)){
                                            $topicInfo["childs"] = [];
                                            foreach ($child as $user_id){
                                                $topicInfo["childs"][] = $user_id;
                                            }    
                                        }
                                        $message[] = $topicInfo;
                                        echo json_encode($message);
                                    }
                                    else {
                                        HTTPStatus("400", "No topic with this ID", "No topic with this ID");
                                    }
                                break;

                            case 'PATCH':
                                if (!empty( $token )){
                                    if (checkIfAdmin($connection, $token) ){
                                        $requestBody = $requestData->body;
                                        $requestName = $requestBody->name;
                                        $requestParentId = $requestBody->parent_id; 
                                        
                                        $patchResult = $connection->query(" UPDATE topics SET name = '$requestName', parent_id = '$requestParentId'  WHERE id ='$topic_id' ");
                                        if ($patchResult){
                                            $topicInfo = [];
                                            $topicInfo["id"] = $topic_id;
                                            $topicInfo["name"] = $requestName;
                                            $topicInfo["parent_id"] = $requestParentId;

                                            $child = $connection->query("SELECT id, name, parent_id FROM topics WHERE parent_id = '$topic_id' ");
                                            if (!is_null($child)){
                                                $topicInfo["childs"] = [];
                                                foreach ($child as $user_id){
                                                    $topicInfo["childs"][] = $user_id;
                                                }    
                                            }
                                            $message[] = $topicInfo;
                                            echo json_encode($message);
                                        }
                                         else {
                                            HTTPStatus("500", "Unexpected Error :(", "Unexpected Error :(");
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

                            case 'DELETE':
                                if (!empty( $token )){
                                    if (checkIfAdmin($connection, $token)){
                                        $givenTopicById = $connection->query("SELECT id FROM topics WHERE id='$topic_id'")->fetch_assoc();
                                        if (!empty($givenTopicById)){
                                            $deleteResult = $connection->query("DELETE FROM topics WHERE id='$topic_id'");
                                            if ($deleteResult){
                                                $message["message"] = "OK";
                                                echo json_encode($message);
                                            }
                                            else {
                                                HTTPStatus("500", "Unexpected Error :(", "Unexpected Error :(");
                                            }
                                        }else{
                                            HTTPStatus("404", "Topic is not exist", "Topic is not exist");
                                        }
                                    }
                                    else{
                                        HTTPStatus("403", "You do not have permission to delete this topic",  "You do not have permission to delete this topic");
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
            //echo "only topics";
            $method = $requestData->method;
            switch ($method){
                case 'GET':
                    if (checkValidToken($connection, $token)){
                        $parameters = $requestData->parameters;
                        $name = $parameters["name"];
                        $parent_id = $parameters["parent_id"];
                        echo json_encode($parameters) . PHP_EOL;

                        if ( !empty($name) && !empty($parent_id) ){
                            $allTopics = $connection->query("SELECT id, name, parent_id from topics WHERE name = '$name' AND parent_id = '$parent_id' ");
                            $topicsArr = [];
                            foreach ($allTopics as $topic_row){
                                $topicsArr[] = $topic_row;
                            }    
                            echo json_encode($topicsArr);
                        }else{
                            if ( !empty($parameters["name"]) ){
                                $allTopics = $connection->query("SELECT id, name, parent_id from topics WHERE name = '$name' ");
                                $topicsArr = [];
                                foreach ($allTopics as $topic_row){
                                    $topicsArr[] = $topic_row;
                                }    
                                echo json_encode($topicsArr);
                            }else{
                                if ( !empty($parameters["parent_id"]) ){
                                    $allTopics = $connection->query("SELECT id, name, parent_id from topics WHERE parent_id = '$parent_id' ");
                                    $topicsArr = [];
                                    foreach ($allTopics as $topic_row){
                                        $topicsArr[] = $topic_row;
                                    }    
                                    echo json_encode($topicsArr);
                                }else{
                                    $allTopics = $connection->query("SELECT id, name, parent_id from topics ");
                                    $topicsArr = [];
                                    foreach ($allTopics as $topic_row){
                                        $topicsArr[] = $topic_row;
                                    }    
                                    echo json_encode($topicsArr);
                                }
                            }
                        }
                    }
                    else{
                        HTTPStatus("403", "You must be logged in", "Your token is not valid");
                    }
                    break;
                case 'POST':
                    if (checkIfAdmin($connection, $token)){
                        $requestBody = $requestData->body;
                        $given_parent_id = $requestBody->parent_id;
                        $given_name = $requestBody->name;
                        echo ($given_name) . PHP_EOL;
                        echo ($given_parent_id) . PHP_EOL;

                        ///////////////create topic function//////////////////
                        $insertNewTopicResult = $connection->query("INSERT INTO topics(name, parent_id) VALUES ('$given_name', '$given_parent_id') ");
                        if ($insertNewTopicResult){
                            ////////////////////print topic function//////////
                            $findParentOfChild = $connection->query("SELECT id, name, parent_id FROM topics WHERE id = '$given_parent_id'  ORDER BY id ASC ")->fetch_assoc();
                            if ($findParentOfChild){
                                $topicInfo = [];
                                $topicInfo["id"] = $findParentOfChild["id"];
                                $topicInfo["name"] = $findParentOfChild["name"];
                                $topicInfo["parent_id"] = $findParentOfChild["parent_id"];

                                $par_id = $findParentOfChild["id"];

                                $child = $connection->query("SELECT id, name, parent_id FROM topics WHERE parent_id = '$par_id' ");
                                if (!is_null($child)){
                                    $topicInfo["childs"] = [];
                                    foreach ($child as $user_id){
                                        $topicInfo["childs"][] = $user_id;
                                    }    
                                }
                                $message[] = $topicInfo;
                                echo json_encode($message);
                            }
                            else{
                                HTTPStatus("500", "Unexpected Error :(", "Unexpected Error :(");
                            }
                            //////////////////////////////////////////////////////////
                        }
                        else{
                            HTTPStatus("500", "Unexpected Error :(", "Unexpected Error :(");
                        }
                        /////////////////////////////////////////////////////
                    }
                    else{
                        HTTPStatus("403", "You do not have permission to view information about this user", "You do not have permission to view information about this user");
                    }
                    break;
                default:
                    HTTPStatus("400", "Bad request", "Not GET/POST method");
                    break;
            }

                
        }
    }
          

    

?>