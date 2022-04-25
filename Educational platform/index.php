<?php

    function getData($method)
    {
        $data = new stdClass();
        $data->method = $method;

        if ($method != "GET")
        {
            $data->body = json_decode(file_get_contents("php://input"));
        }


        $data->parameters = [];
            $dataGet = $_GET;
            foreach ($dataGet as $key => $value)
            {
                if ($key != "q")
                {
                    $data->parameters[$key] = $value;
                }
            }
            
        return $data;
    }

    function getMethod()
    {
        return $_SERVER["REQUEST_METHOD"];
    }

    header("Content-type: application/json");

    $connection = mysqli_connect("127.0.0.1", "admin", "password", "database");
    
    if (!$connection)
    {
        echo "Невозможно установить соединение с MySql" . PHP_EOL;
        echo "Код ошибки: " . mysqli_connect_errno() . PHP_EOL;
        echo "Текст ошибки: " . mysqli_connect_error() . PHP_EOL;
        exit;
    }
    else
    {
        //echo 'connect is succesful';
    }

    /*
    $message = [];
    $message["users"] = [];
    $res = $connection->query("SELECT User_ID, Name, Username FROM users ORDER BY id ASC");

    if (!$res)
    {
        echo "Не удалось выполнить запрос: (" . $mysqli->errno . ") " . $mysqli->error;
    }
    else{
        while ($row = $res->fetch_assoc())
        {
            $message["users"][] =
            [
                "id" => $row["id"],
                "login" => $row["login"],
                "name" => $row["name"],
            ];
        }
    }
    */

    $url = isset($_GET["q"]) ? $_GET["q"] : '';
    $url = rtrim($url, '/');
    $urlList = explode('/', $url);

    $router = $urlList[0];
    $urlList = array_slice($urlList, 1);
    $requestData = getData(getMethod());

    //echo $router . PHP_EOL;
    //echo json_encode($urlList) . PHP_EOL;
    //echo json_encode($requestData);

    if (file_exists(realpath(dirname(__FILE__))."/routers/" .$router . ".php" ))
    {
        include_once "routers/" . $router . ".php";
        route($connection, $urlList, $requestData);
    }
    else
    {
        //echo "404, file not found";
    }
?>
