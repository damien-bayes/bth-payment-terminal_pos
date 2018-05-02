<?php
/**
 * Loading templates
 *
 * @author Bayesian Workgroup and Network
 */
namespace Bayesian\Mvc;

class View {
  protected $properties;

  private $tplFile;

  /**
   * Constructor
   *
   * @var string
   */
  public function __construct($tplFile) {
    if (isset($tplFile)) {
      if (is_string($tplFile) && file_exists($tplFile)) {
        $this->tplFile = $tplFile;
        $this->properties = array();
      }
    }
  }

  /**
   *
   *
   * @var object
   * @var object
   */
  public function replace($key, $value) {
    $this->properties[$key] = $value;
  }

  /**
   *
   * Extended patterns:
   * {@title='<Name>'} = {@title=(.[^"]+)}
   */
  public function render() {
    // Gets the main layout from main.tpl
    $main = file_get_contents(__BASEURL__ . '/views/layouts/main.tpl');

    // Gets the additional view from <requested>.tpl
    $content = file_get_contents($this->tplFile);

    // {@stylesheets}...{@stylesheets}
    $main = $this->extract('stylesheets', $content, $main);
    // {@content}...{@content}
    if (strpos($this->tplFile,'settings') !== false) {
      $settings = file_get_contents(__BASEURL__ . '/views/layouts/settings.tpl');

      $main = str_replace('{@content}', $settings, $main);
    }
    $main = $this->extract('content', $content, $main);
    // {@scripts}...{@scripts}
    $main = $this->extract('scripts', $content, $main);

    /*
     * Regex for: {@<key>="<value>"}
     * Issues: preg_replace_callback
     *
     * $data[0] - Exact match
     * $data[1] - Values encased in the first matching
     * $data[2] - Values encased in the second matching
     */
    $hitNumber = preg_match_all('#{@([\w]+)="([^"]+)"}#', $main, $data);

    if ($hitNumber > 0) {
      for ($i=0; $i < $hitNumber; $i++) {
        switch ($data[1][$i]) {
          case 'mcrypt':
            $version = '.' . md5(filemtime(substr_replace(__BASEURL__, '', -1) . $data[2][$i])) . '.';

            $url = pathinfo($data[2][$i]);
            $to = $url['dirname'] . __DS__ . $url['filename'] . $version . $url['extension'];

            $main = str_replace($data[0][$i], $to, $main);
            break;
        }
      }
    }

    // $indexView->replace('title', 'Main Page');
    foreach ($this->properties as $key => $value) {
      if (!is_array($value)) {
        $tagToReplace = "{@$key}";
        $main = str_replace($tagToReplace, $value, $main);
      }
      else {
        $hitNumber = preg_match_all('#{@([\w]+)="([^"]+)"}#', $main, $data);

        if ($hitNumber > 0) {
          for ($i=0; $i < $hitNumber; $i++) {
            switch ($data[1][$i]) {
              case 'foreach':

                preg_match_all('#{@foreach="([^"]+)"}([^"]+){@endforeach}#', $main, $foreachData);

                $result = '';
                $tmp;

                foreach ($this->properties[$data[2][$i]] as $object) {
                  $object = get_object_vars($object);

                  $count = preg_match_all('#{@([\w]+)}#', $foreachData[2][0], $data3);

                  $tmp = $foreachData[2][0];

                  for ($j=0; $j < $count; $j++) {
                    $tmp = str_replace($data3[0][$j], $object[$data3[1][$j]], $tmp);
                  }

                  $result .= $tmp;
                }

                $main = str_replace('{@foreach="banners"}', '', $main);
                $main = str_replace('{@endforeach}', '', $main);
                $main = str_replace($foreachData[2][0], $result, $main);

                break;

            }
          }
        }
      }
    }

    return $main;
  }

  /**
   * Boyer-Moore string search algorithm
   *
   * @param text - The string to be searched
   * @param pattern - The target string to be searched
   * @return The start index of the substring
   */
  function searchIndex($text, $pattern) {
    $patternLength = strlen($pattern);
    $textLength = strlen($text);
    $table = $this->makeCharacterTable($pattern);

    for ($i = $patternLength - 1; $i < $textLength;) {
      $t = $i;
      for ($j = $patternLength - 1; $pattern[$j] == $text[$i]; $j--, $i--) {
        if ($j == 0) {
          return $i;
        }
      }
      $i = $t;
      if (array_key_exists($text[$i], $table)) {
        $i = $i + max($table[$text[$i]], 1);
      }
      else {
        $i += $patternLength;
      }
    }
    return -1;
  }

  function makeCharacterTable($string) {
    $length = strlen($string);
    $table = array();

    for ($i = 0; $i < $length; $i++) {
      $table[$string[$i]] = $length - $i - 1;
    }

    return $table;
  }

  /*
   *
   * @param string $key - {@content}...{@content}, {@scripts}...{@scripts}
   * @param string $from - view
   * @param string $to - layout
   */

  private function extract($key, $from, $to) {
    // /{@<Key>}([^$]*){@<Key>}/
    //$pattern = '/{@' . $key . '}([^$]*){@' . $key . '}/';
    $pattern = '/{@' . $key . '}([^~]*){@' . $key . '}/';

    $success = preg_match_all($pattern, $from, $matches);
    if ($success) {
      // Removes defined tags to replace (For example: {@content}...{@content})
      $matches = str_replace('{@' . $key . '}', '', $matches[0]);

      return str_replace('{@' . $key . '}', $matches[0], $to);
    }

    return NULL;
  }
}
?>
