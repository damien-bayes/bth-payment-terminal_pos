  <!-- -->
{@stylesheets}
<link rel='stylesheet' type='text/css' href='{@mcrypt="/css/feedback.css"}' /> {@stylesheets} {@content}

<section class="main-central-deck" style="position: static;">
  <div class='feedback-success'>
    <div class='icon'>
      <span>+1</span>
      <img src='/images/feedback.svg' />
    </div>
    <div class='content'>
      <h3>Спасибо за отзыв</h3>
      <p>Ваш отзыв принят и будет опубликован в ближайшее время!</p>
      <a class='button primary-button' href='javascript: window.location.href = "/"'>Вернуться</a>
    </div>
    <!--<a href='#'>Посмотреть статистику отзывов</a>-->
  </div>
</section>
{@content} {@scripts}
<script src='{@mcrypt="/js/idle.min.js"}'></script>

<script type='text/javascript'>
  document.addEventListener('DOMContentLoaded', _ => {
    new Idle({
      delay: 120000, // Give 2 minutes before returning to the homepage
      onExpire: () => {
        window.location.href = '/'; // Return to the homepage on expiring
      }
    });
  });
</script>
{@scripts}
