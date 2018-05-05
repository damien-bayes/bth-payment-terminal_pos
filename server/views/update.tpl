{@stylesheets}
<link rel='stylesheet' type='text/css' href='{@mcrypt="/css/feedback.css"}' />
<style>
  @keyframes zoomIn {
    from {
      opacity: 0;
      transform: scale3d(.3, .3, .3);
    }

    50% {
      opacity: 1;
    }
  }
</style>
{@stylesheets} {@content}
<section class="main-central-deck" style="position: static;">
  <div class='feedback-success'>
    <div class='icon' style="background-color: #390b11;">
      <span id="version" style="font-size: 80%;"></span>
      <i class='fa fa-cog fa-4x fa-spin'></i>
    </div>
    <div class='content'>
      <h6 id='phrase' style="animation: zoomIn .3s linear; font-weight: bold; margin: 1em 0; color: #575c67;">Подготовка к обновлению...</h6>
      <h3 id='percentage'>0%</h3>
      <p style="color: #9da0a5;" id='message'>&nbsp;</p>
    </div>
  </div>
</section>
{@content} {@scripts}
<script>
  ((w, d) => {
    'use strict';

    let originalArray, totalKeys = [];

    function phrase(array = []) {
      originalArray = array;
    };

    phrase.prototype = {
      spin: _ => {
        const key = Math.floor(Math.random() * originalArray.length);

        if (totalKeys.indexOf(key) == -1) {
          totalKeys.push(key);

          if (totalKeys.length == originalArray.length) totalKeys = [];

          return originalArray[key];
        } else phrase.prototype.spin(this);
      }
    }

    w.Phrase = phrase;
  })(window, document);

  document.addEventListener('DOMContentLoaded', _ => {
    const phrases = [
      'Это не займет много времени',
      'Идет обновление, не выключайте терминал',
      'Терминал постоянно обновляется, чтобы улучшить качество ПО',
      'Мы рады, что вы с нами',
      'Получение критических обновлений'
    ];
    const p = new Phrase(phrases);

    setInterval(_ => {
      const phrase = p.spin();
      const element = document.getElementById('phrase');
      element.innerHTML = typeof phrase != 'undefined' ? phrase : phrases[Math.floor(Math.random() * phrases.length)];

      const renewedElement = element.cloneNode(true);
      element.parentNode.replaceChild(renewedElement, element);
    }, 5000);

    fetch('https://ipts.agor.kz/update/version.txt').then(function(response) {
      return response.text();
    }).then(function(data) {
      const version = data.split(';')[0];
      document.getElementById('version').innerText = `Version ${version}`;
    });

    updateObject.startUpdating();
  });
</script>
{@scripts}
