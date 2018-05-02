<?php
require_once(__DIR__ . '/../bayesian/class.model.php');

class Agent extends \Bayesian\Mvc\Model {
  public function __construct() {
    parent::__construct();
  }

  public function validatePincode($arguments = []) {
    // Verify user's accessibility
    if (!parent::verifyToken()) {
      throw new \Exception("Access denied", 1);
    }

    $arguments = array_map('htmlspecialchars', $arguments);

    $pincode = $arguments['pincode'];
    $prefix = $arguments['prefix'];

    $isGlobal = false;

    if (strpos($pincode, '@') !== false) {
      $pincode = str_replace('@', '', $pincode);
      $isGlobal = true;
    }

    if (!is_numeric($pincode . $prefix)) {
      throw new \Exception("Password not numeric", 1);
    }

    if (substr($pincode, 0, 3) != $prefix) {
      throw new \Exception("Pattern matching error", 1);
    }

    /*
     * Is Used To Destroy All Sessions
     */
    //session_destroy();

    /*if(isset($_SESSION['agentId'])) {
      unset($_SESSION['agentId']);
    }*/

    foreach($_SESSION as $k=>$v){
      unset($_SESSION[$k]);
    }
    @session_unset();
    @session_destroy();
    @session_regenerate_id(true);
    @session_write_close();

    if (!$isGlobal) {
      $query = 'SELECT id, taxpayerNumber FROM agents WHERE id = (SELECT agentId FROM hotelStations WHERE hotelId = (SELECT hotelId FROM terminals WHERE token = :token))';
      $result = $this->db->query($query, array(
        'token' => $_SERVER['HTTP_AUTHORIZATION']
      ));

      $pincode = substr($pincode, 3, strlen($pincode) - 1);
      if ($pincode != $result[0]['taxpayerNumber']) {
        throw new \Exception("The specified terminal password is not correct", 1);
      }
      else {
        session_start();
        $_SESSION['agentId'] = $result[0]['id'];
        session_write_close();

        return true;
      }
    }
    else {
      $pincode = substr($pincode, 3, strlen($pincode) - 1);

      $query = 'SELECT id FROM agents WHERE taxpayerNumber = :taxpayerNumber';
      $result = $this->db->query($query, array(
        'taxpayerNumber' => $pincode
      ));

      if (!$result) {
        throw new \Exception("The specified terminal password is not correct", 1);
      }

      session_start();
      $_SESSION['agentId'] = $result[0]['id'];
      session_write_close();

      return true;
    }

    return false;
  }
}
