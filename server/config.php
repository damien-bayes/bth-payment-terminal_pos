<?php
/**
 * Config
 */
$config = [
  /**
   * Web application name
   */
  'appName' => 'IPTS',

  /**
   * Web application version
   */
  'version' => '1.0.0.0',

  /**
   * Debug mode
   */
  'debug' => true,

  /**
   * Default language
   */
  'language' => 'ru',

  /**
   * Geocoding
   */
   'geocoding' => [
     'key' => ''
   ],

  /**
   * Database configuration
   * This is the place where we define our database credentials, database types and etc.
   */
  'db' => [
    'dsn' => 'mysql:host=127.0.0.1;dbname=YOUR_DB_NAME;charset=utf8',
    'username' => '',
    'password' => ''
  ],

  /**
   * Cookie configuration
   */
  'cookie' => [
    'key' => 'secret.key',
    'expires' => time() + 60 * 5, // 30 seconds
    'path' => '/',
    'domain' => '',
    'secure' => '',
    'httponly' => ''
  ],

  /**
   * SMTP configuration
   */
  'smtp' => [
    'host' => '',
    'username' => '',
    'password' => '',
    'port' => 465,
    'from' => array(
      'email' => '',
      'show'  => 'IPTS'
    )
  ]
];
