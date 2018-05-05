{@stylesheets}{@stylesheets} {@content}
<!-- Feedback -->
<section class="main-central-deck" style="position: static;">
  <h2 class='service-title'><i class="fa fa-comment"></i>Отзывы</h2>

  <form id='feedbackForm' method='post' style="width: 700px;">
    <div class='form-body'>
      <!-- 1 -->
      <label class='form-label' for='rating1'>Средняя оценка</label>
      <div class='rating' id='rating1'></div>

      <!-- 2 -->
      <label class='form-label' for='message'>Текст отзыва (не обязательно)</label>
      <input class='form-control' type='text' id='message' name='message'>

      <!--<a href='/feedback/statistics'>Посмотреть статистику отзывов</a>-->
    </div>

    <div class='form-footer'>
      <button id='publish' type='submit' class='button primary-button large-button' title='Опубликовать'>Опубликовать</button>
    </div>

    <p style='text-align: center;' class='zoomIn' id='error'>&nbsp;</p>
  </form>
</section>
<!-- END: Feedback -->
{@content} {@scripts}
<!-- Feedback scripts -->
<script src='{@mcrypt="/js/idle.min.js"}'></script>
<script src='{@mcrypt="/js/rating.js"}'></script>
<script src='{@mcrypt="/js/keyboard.min.js"}'></script>
<script src='{@mcrypt="/js/cookie.js"}'></script>

<script type='text/javascript'>
  document.addEventListener('DOMContentLoaded', _ => {
    new Idle({
      delay: 120000, // Give 2 minutes before returning to the homepage
      onExpire: () => {
        window.location.href = '/'; // Return to the homepage on expiring
      }
    });

    let rating1 = new Rating({
      target: document.getElementById('rating1'),
      markup: '<i class="fa fa-fw fa-5x fa-star"></i>'
    });
    rating1.initialize();

    new Keyboard(document.getElementById('message'), {
      position: 'top center',
      language: 'ru-RU'
    });

    const form = document.getElementById('feedbackForm');
    form.addEventListener('submit', (e) => {
      e.preventDefault();

      document.getElementById('publish').disabled = true;

      if (Cookie.read('feedback-timeout') === null) {
        callApi('feedback.sendMessage', new FormData(form)).then(response => {
          Cookie.create('feedback-timeout', '', 1);
          window.location.href = '/feedback/success';
        });
      } else {
        document.getElementById('error').innerText = 'Вы сможете оставить следующий отзыв в течении 1 минуты.'
        document.getElementById('publish').disabled = false;
      }
    });
  });
</script>
<!-- END: Feedback scripts -->
{@scripts}
