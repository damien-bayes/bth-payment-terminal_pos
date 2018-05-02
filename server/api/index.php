<?php
class Response {
  private $status;
  private $data;

  public function __construct() {
    $this->status = false;
    $this->data = "No data";
  }

  public function setResponse(Response $response): Response {
    $this->status = $response->status;
    $this->data = $response->data;

    return $this;
  }

  public function setStatus(bool $status): Response {
    $this->status = $status;
    return $this;
  }

  public function setData($data): Response {
    $this->data = $data;
    return $this;
  }

  public function getJson(): string {
    return json_encode(array(
              'status' => $this->status ? "success" : "error",
              'data'   => $this->data
    ), JSON_UNESCAPED_UNICODE);
  }
}

// Send a raw HTTP header
header('Content-Type: application/json; charset=UTF-8');

$parts = explode('/', $_SERVER['REQUEST_URI'])[2]; // [0]=>'', [1]=>'api', [2]=>'feedback.getById and etc.'
$parts = explode('.', $parts); // [0]=>'feedback', [1]=>getById

// Escaping odds and ends
$parts = preg_replace('/([^A-Za-z])+/', '', $parts);

// Determining the names
$className = strtolower($parts[0]);
$methodName = $parts[1];

require_once(__DIR__ . '/../models/' . $className . '.php'); // /../models/feedback.php

$className = ucfirst($className); // Feedback

if (class_exists($className)) {
  $x = new $className();

  if (method_exists($x, $methodName)) {
    $response = NULL;

    try {
      $response = (new Response())
        ->setStatus(true)
        ->setData($x->$methodName($_POST))
        ->getJson();
    }
    catch(Exception $e) {
      $response = (new Response())
        ->setStatus(false)
        ->setData($e->getMessage())
        ->getJson();
    }

    echo $response;
  }
}
?>
