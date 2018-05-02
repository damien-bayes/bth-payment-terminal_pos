<?php
require_once(__DIR__ . '/../bayesian/class.model.php');
require_once(__DIR__ . '/../bayesian/helpers/class.utils.php');

use Bayesian\Helpers\Utils;

class Terminal extends \Bayesian\Mvc\Model {
  public function __construct() {
    parent::__construct();
  }

  public function encash() {
    // Verify user accessibility
    if (!parent::verifyToken()) {
      throw new \Exception("Access denied", 1);
    }

    $query = 'SELECT sum FROM terminals WHERE token=:token';
    $result = $this->db->query($query, array(
      'token' => $_SERVER['HTTP_AUTHORIZATION']
    ));

    $sum = $result[0]['sum'];

    if (intval($sum) <= 0) {
      throw new \Exception("Your terminal is empty.", 1);
    }

    session_start();

    $this->db->beginTransaction();

    $query = 'INSERT INTO encashments (agentId, sum, terminalId) VALUES (:agentId, :sum, (SELECT id FROM terminals WHERE token = :token))';
    $result = $this->db->query($query, array(
      'agentId' => $_SESSION['agentId'],
      'sum' => $result[0]['sum'],
      'token' => $_SERVER['HTTP_AUTHORIZATION']
    ));

    $query = 'UPDATE terminals SET sum=0 WHERE token=:token';
    $result = $this->db->query($query, array(
      'token' => $_SERVER['HTTP_AUTHORIZATION']
    ));

    $query = 'SELECT LAST_INSERT_ID() AS encashmentId FROM encashments';
    $result = $this->db->query($query);

    $this->db->commit();

    return $result[0];
  }

  /**
   * Register a new terminal in the database and receiving a token
   */
  public function register($args = []) {
    // Identifies essential $_POST values
    $keys = ['guid', 'username', 'machineName', 'operatingSystemVersion'];

    // Checking for existence in an array
    if (array_diff($keys, array_keys($args))) {
      throw new Exception("ArgumentNullException: The exception that is thrown when a null reference is passed to a method that does not accept it as a valid argument.");
    }

    // Convert special characters to HTML entities
    $args = array_map(function($element) {
      return htmlspecialchars($element, ENT_QUOTES, 'UTF-8');
    }, $args);

    // Calculate the md5 hash of a string
    $guid = md5($args['guid']);

    // Creates a random token which length is 100
    $token = bin2hex(random_bytes(100));

    // First MySQL query pattern
    $query = 'INSERT INTO terminals (guid, token, username, machineName, operatingSystemVersion)
              VALUES (:guid, :token, :username, :machineName, :operatingSystemVersion)';

    // Executing
    $this->db->query($query, array(
      'guid'=>$guid,
      'token'=>$token,
      'username'=>$args['username'],
      'machineName'=>$args['machineName'],
      'operatingSystemVersion'=>$args['operatingSystemVersion']
    ));

    // Second MySQL query pattern
    $query = 'SELECT verified, token FROM terminals WHERE guid=:guid';

    // Executing and returning a response from the database
    $response = $this->db->query($query, array(
      'guid'=>$guid
    ));

    return $response[0];
  }

  /**
   * Login a terminal in the database and receiving a token
   */
  public function login($args = []) {
    // Identifies essential $_POST values
    $keys = ['guid'];

    // Checking for existence in an array
    if (array_diff($keys, array_keys($args))) {
      throw new Exception("ArgumentNullException: The exception that is thrown when a null reference is passed to a method that does not accept it as a valid argument.");
    }

    // Convert special characters to HTML entities
    $args = array_map(function($element) {
      return htmlspecialchars($element, ENT_QUOTES, 'UTF-8');
    }, $args);

    // Calculate the md5 hash of a string
    $guid = md5($args['guid']);

    // First MySQL query pattern
    $query = 'UPDATE terminals SET launchDate=CURRENT_TIMESTAMP WHERE guid=:guid';

    $this->db->query($query, array(
      'guid'=>$guid
    ));

    // Second MySQL query pattern
    $query = 'SELECT token FROM terminals WHERE guid=:guid';

    // Executing and returning a response from the database
    $response = $this->db->query($query, array(
      'guid'=>$guid
    ));

    return $response[0];
  }

  public function getById() {

  }
}
