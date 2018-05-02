<?php
namespace Bayesian\Mvc;

abstract class Controller {
  public function __construct() {
    $arguments = array_map(function($argument) {
      return htmlspecialchars($argument, ENT_QUOTES, 'UTF-8');
    }, $_GET);

    if (!isset($arguments['page'])) {
      $arguments['page'] = 'index';
    }

    $functionName = 'action' . ucwords($arguments['page']);

    if (method_exists(get_class($this), $functionName)) {
      unset($arguments['page']);

      $this->$functionName($arguments);
    }
    else {
      header('Location: /');
    }
  }
}
