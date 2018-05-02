<?php
/**
 * Database workstation = Database helper
 *
 * @author Bayesian Workgroup and Network
 *
 * @version 1.0.0.0
 *
 * @example
 * $db = new Bayesian\Helpers\Database(<dsn>, <username>, <password>);
 * $result = $db->select('SELECT * FROM feedback WHERE id = :id', array('id' => 2));
 */

namespace Bayesian\Helpers;

// Represents a connection between PHP and a database server
use PDO;

class Database extends PDO {
  /**
    * Data source name
    */
  private $_dsn;

  /**
   * Username
   */
  private $_username;

  /**
   * Password
   */
  private $_password;

  /*
   * Php data object
   */
  private $_pdo = NULL;

  /**
   * Database connection status
   */
  private $_isConnected = false;

  /**
   * Constructor
   *
   * @param string $dsn
   * @param string $username
   * @param string $password
   */
  public function __construct($dsn, $username, $password) {
    $this->_dsn = $dsn;
    $this->_username = $username;
    $this->_password = $password;
  }

  /*
   * Connect
   */
  private function connect() {
    if (!$this->_isConnected) {
      try {
        $options = array(PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_OBJ);

        $this->_pdo = new PDO($this->_dsn,  $this->_username, $this->_password, $options);
        // Log any exceptions on fatal errors and etc.
        //$this->_pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        //$this->_pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_WARNING);

        // Disable emulation of prepared statements.
        $this->_pdo->setAttribute(PDO::ATTR_EMULATE_PREPARES, true);

        $this->_isConnected = true;
      }
      catch(PDOException $exception) {
        throw new Exception($exception->getMessage());
        die();
      }
    }
  }

  /**
   * Disconnect
   */
  private function disconnect() {
    if (!is_null($this->_pdo)) {
      $this->_pdo = null;
      $this->_isConnected = false;
    }
  }

  /**
   * Selecting records from a database
   *
   * @param string $query
   * @param array $parameters
   * @return array
   */
  public function select($query, $parameters = []) {
    $result = NULL;

    // Check database connection status
    if (!$this->_isConnected) {
      $this->connect();
    }

    try {
      $stmt = $this->_pdo->prepare($query);

      // Bind input parameters to the query
      foreach ($parameters as $key => $value) {
        if (is_int($value)) {
          $stmt->bindValue("$key", $value, PDO::PARAM_INT);
        }
        $stmt->bindValue("$key", $value);
      }

      // Start query execution
      $stmt->execute();

      // Fetching all data as an array
      $result = $stmt->fetchAll();
    }
    catch(PDOException $exception) { }

    $this->disconnect();

    return $result;
  }

  /**
   * Updates a table in the database
   *
   * @param string $table
   * @param array $data
   * @param array $where
   * @return int
   */
  public function update($table, $data = [], $where = []) {
    $result = 0;

    // Check database connection status
    if (!$this->_isConnected) {
      $this->connect();
    }

    $data = $this->updateHelper($data);
    $where = $this->updateHelper($where, 'AND');

    try {
      $stmt = $this->_pdo->prepare("UPDATE $table SET $data WHERE $where");

      // Start query execution
      $stmt->execute();

      $result = $stmt->rowCount();
    }
    catch(PDOException $exception) { }

    $this->disconnect();

    return $result;
  }

  /**
   * Goes through an array and generates a string for the update query
   *
   * @param array $array
   * @param string $separator
   * @return string
   */
  private function updateHelper($array, $separator = ',') {
    // Sort an array by key
    ksort($array);

    $x = (string)NULL;
    foreach ($array as $key => $value) {
      $x .= $key . '=\'' . $value . '\' ' . $separator . ' ';
    }
    $x = rtrim(trim($x), $separator);

    return $x;
  }

  /**
   *
   */
  public function delete($table, $where, $limit = 1) {
    // Check database connection status
    if (!$this->_isConnected) {
      $this->connect();
    }
  }
}
?>
