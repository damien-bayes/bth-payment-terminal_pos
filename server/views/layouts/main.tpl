<!DOCTYPE html>
<html lang="ru">

<head>
  <meta charset="utf-8" />
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

  <meta name="language" content="ru" />
  <meta name='robots' content="noindex, nofollow" />

  <meta name="author" content="Damien Bayes, damien.bayes.db@gmail.com" />
  <meta name="copyright" content="Bayesian Workgroup and Network" />

  <title>IPTS - {@title}</title>

  <!-- Stylesheets (1 - Global stylesheets, 2 - Local stylesheets) -->
  <!-- 1 -->
  <link href="https://fonts.googleapis.com/css?family=PT+Sans+Narrow" rel="stylesheet">

  <link rel="stylesheet" type="text/css" href='{@mcrypt="/css/vendors/font-awesome-4.7.0/font-awesome.min.css"}' />
  <link rel="stylesheet" type="text/css" href='{@mcrypt="/css/animations.css"}' />
  <link rel="stylesheet" type="text/css" href='{@mcrypt="/css/style.css"}' />
  <!-- 2 -->
  {@stylesheets}
  <!-- END: Stylesheets -->
</head>

<body>
  <!-- Loader -->
  <div class='loader-backdrop' id='loader1'>
    <div class='loader'>
      <div class='box'></div>
      <div class='walkway'></div>
      <p data-action='loader-message'>Загрузка, пожалуйста подождите...</p>
    </div>
  </div>
  <!-- END: Loader -->

  <header>
    <!-- Taskbar -->
    <div class='taskbar' id='taskbar1'>
      <!-- <a style='text-align: center;' href='javascript:'>Сборка: 1.0.3.9679</a> -->
      <div class='taskbar-brand'>
        <img data-action='taskbar-stash' src='/images/brand.svg' alt='Inn24' />
      </div>
      <ul>
        <li>
          <a class="taskbar-item" href='/'><i class="fa fa-home"></i>Главная</a>
        </li>
        <li>
          <a class='taskbar-item' href='javascript: location.reload();'><i class="fa fa-refresh"></i>Обновить</a>
        </li>
        <li class="divider"></li>
        <!--
        <li>
          <a class='taskbar-item' href='javascript: location.reload();'><i class="fa fa-file"></i>Статус платежа</a>
        </li>-->
        <!--<li>
          <a class='taskbar-item' href='/guidebook'><i class="fa fa-fw fa-map"></i></a>
        </li>-->
        <!--<li>
          <a data-action='feedback' class='taskbar-item' href='/feedback/add'><span data-count='0'><i class="fa fa-comment"></i></span>Отзывы</a>
        </li>-->
      </ul>
      <ul class="taskbar-menu-right">
        <li class='agent-box' style="display: flex; justify-content: center; align-items: center; padding: 0 1em;">
          <div class="agent-details" style="pointer-events: none;display: flex; flex-wrap: wrap; flex-direction: column; justify-content: center; align-items: flex-end;">
            <h3 id="hotelName" style="pointer-events: none;padding: 0;font-size: 18px; text-transform: uppercase; font-weight: 400; color: #404042;">Доступ запрещен</h3>
            <p style="font-size: 80%; color: #404042;">Идентификатор:<span id="hotelId" style="pointer-events: none; margin-left: .25em;">0</span></p>
          </div>
          <img id="hotelAvatar" width="48" height="48" style="pointer-events: none;border-radius: 50%; margin: 0 .5em;" />
        </li>
      </ul>
    </div>
    <!-- END: Taskbar -->
  </header>

  <main>{@content}</main>

  <footer>
    <div class="copyright" id="copyright"></div>
    <ul class="links">
      <li><a href="javascript:">Конфиденциальность</a></li>
      <li><a href="/agreements">Пользовательское соглашение</a></li>
      <li><a href="#" id="advRules">Рекламные объявления</a></li>
    </ul>
  </footer>

  <!-- Scripts: 1 - Default scripts, 2 - Imported scripts -->
  <!-- 1 -->
  <script src='{@mcrypt="/js/api.js"}'></script>
  <script src='{@mcrypt="/js/taskbar.js"}'></script>
  <script src='{@mcrypt="/js/tooltips.min.js"}'></script>
  <script src='{@mcrypt="/js/modal.min.js"}'></script>

  <script type='text/javascript'>
    document.addEventListener('keyup', e => {
      if (e.ctrlKey && e.shiftKey && e.code === 'KeyX') {
        customObject.showDevTools();
      }
    });

    document.addEventListener('DOMContentLoaded', _ => {
      if (typeof updateObject !== 'undefined') {
        document.getElementById('copyright').innerText = `© Copyright 2017-${new Date().getFullYear()} Version: ${updateObject.currentVersion}`;
      }

      callApi('hotel.getName').then(response => {
        document.getElementById('hotelName').innerText = response.data.hotelName;
        document.getElementById('hotelId').innerText = response.data.id;

        document.getElementById('hotelAvatar').src = response.data.imageUrl;
        document.getElementById('hotelAvatar').alt = response.data.caption;
      }).catch(error => { console.log(error)});

      let taskbar1 = new Taskbar('#taskbar1', {
        clickCount: 5
      }, _ => {
        window.location.href = '/login/extra';
      }).initialize();

      new Tooltips('[data-tooltip]', {}).initialize();

      try {
        if (billValidatorObject && typeof billValidatorObject !== undefined) {
          billValidatorObject.powerUp();
        }
      } catch (e) {}

      new Modal(document.getElementById('advRules'), {
        caption: 'Правила размещения рекламных объявлений',
        message: 'Ознакомление с рекламными объявлениями можно будет осуществить в ближайщее время.',
        onClickState: (self, instance, status) => {
          if (status === 'ok') self.close(instance);
        }
      });
    });

    window.addEventListener('load', () => {
      document.getElementById('loader1').remove();
    });
  </script>

  <!-- 2 -->
  {@scripts}
  <!-- END: Scripts -->
</body>

</html>
