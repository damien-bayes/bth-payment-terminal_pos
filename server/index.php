<?php
define('__LAUNCHTIME__', microtime(true));

// Set timezone
date_default_timezone_set('Asia/Almaty');

defined('__DS__') || define('__DS__', DIRECTORY_SEPARATOR);

define('__BASEURL__', __DIR__ . __DS__);
define('__CONFIGFILEDIR__', __DIR__ . __DS__ . 'config.php');

if (!file_exists(__CONFIGFILEDIR__)) {
  throw new Exception('FileNotFoundException. The exception that is thrown when an attempt to access a file that does not exist on disk fails.');
  die();
}

// Require configuration (error reporting, database configuration and etc.)
require(__BASEURL__ . 'config.php');

// Check for valid config
if (!is_array($config)) {
  throw new Exception('NullReferenceException. Parameter should be an Array.');
  exit();
}

if(isset($config['debug']) && $config['debug'] === true) {
  /**
    * Error reporting configuration
    * Useful to show every little problem during development.
    */
  error_reporting(E_ALL);
  ini_set('display_errors', 1);
}
else {
  error_reporting(0);
}

require(__BASEURL__ . '/controllers/siteController.php');
use Bayesian\Controller\SiteController;

$controller = new SiteController();

//require(__BASEURL__ . 'bayesian/helpers/class.geocoding.php');
//use Bayesian\Helpers\Geocoding;
//$latitudeAndLongitude = Geocoding::getCoordinates(array(null, 'Kazakhstan', null, null), $config['geocoding']['key']);

define('__ENDTIME__', microtime(true));
//var_dump(__ENDTIME__ - __LAUNCHTIME__);
