{@stylesheets} {@stylesheets} {@content}
<!-- Reservation -->
<section class="main-central-deck" style="position: static;">
  <h2 class='service-title' style="animation: fadeInLeft 1s;"><i class="fa fa-sign-in"></i>Регистрация</h2>

  <form id='registrationForm' method='post' style="width: 700px;">
    <div data-section='0'>
      <div class='form-body'>
        <label class='form-label' for='fullname'>Введите фамилию, имя, отчество</label>
        <input class='form-control' type='text' id='fullname' name='fullname' placeholder='Иванов Иван Иванович' />

        <ul class='radio'>
          <li>
            <input id='male' class='radio-button' checked type='radio' value="0" name='gender' />
            <div class='content'>
              <img src='https://ipts.agor.kz/images/male.svg' style="width: 64px;" />
              <label for='male' class='label'>Мужской</label>
            </div>
          </li>
          <li>
            <input id='female' class='radio-button' type='radio' value="1" name='gender' />
            <div class='content'>
              <img src='https://ipts.agor.kz/images/female.svg' style="width: 64px;" />
              <label for='female' class='label'>Женский</label>
            </div>
          </li>
        </ul>

        <label class='form-label' for='phone'>Введите номер сотового телефона</label>
        <input class='form-control' type='number' id='phone' name='phone' placeholder='87771234567' />

        <label class='form-label' for='email'>Введите адрес электронной почты</label>
        <input class='form-control' type='text' id='email' name='email' placeholder='you@example.com' />

      </div>
      <div class='form-footer'>
        <button class='button primary-button large-button' data-action='next'>Далее</button>
      </div>
    </div>

    <div data-section='1'>
      <div class='form-body'>
        <label class='form-label' for='taxpayerIdNumber'>Введите ИИН/БИН</label>
        <input class='form-control' placeholder='123456789012' type='number' id='taxpayerIdNumber' name='taxpayerIdNumber' />

        <label class='form-label' for='idCard'>Введите № удостоверения личности</label>
        <input class='form-control' placeholder='123456789' type='number' id='idCard' name='idCard' />

        <label class='form-label' for='taxpayerIdNumber'>Кем и когда выдано удостоверение личности</label>
        <div class='form-row'>
          <div class='form-column'>
            <select class='form-control' name="extradition">
              <option value="0">МВД РК</option>
              <option value="1">МЮ РК</option>
            </select>
          </div>
          <div class='form-column'>
            <div class='input-group'>
              <div class='input-group-addon'><span class='fa fa-calendar'></span></div>
              <input class='form-control' style="margin-bottom: .5em;" placeholder="гггг/мм/дд" id='idCardIssueDate' name='idCardIssueDate' />
            </div>
          </div>
        </div>
        <ul class="errorList" id="errorList"></ul>
      </div>
      <div class='form-footer'>
        <button class='button large-button' data-action='back'>Назад</button>
        <button class='button primary-button large-button' type="submit">Зарегистироваться</button>
      </div>
    </div>

  </form>
</section>
<!-- END: Reservation -->
{@content} {@scripts}
<script src='{@mcrypt="/js/idle.min.js"}'></script>
<script src='{@mcrypt="/js/keyboard.min.js"}'></script>
<script src='{@mcrypt="/js/multiform.js"}'></script>
<script src='{@mcrypt="/js/datepicker.min.js"}'></script>

<script>
  document.addEventListener('DOMContentLoaded', _ => {
    new Idle({
      delay: 120000, // Give 2 minutes before returning to the homepage
      onExpire: () => {
        window.location.href = '/'; // Return to the homepage on expiring
      }
    });

    const registrationForm = document.getElementById('registrationForm');
    new Multiform(registrationForm).initialize();

    new Keyboard(document.getElementById('fullname'), {
      position: 'bottom center',
      language: 'ru-RU',
      uppercase: false,
    });

    new Keyboard(document.getElementById('phone'), {
      position: 'top center',
      numpad: true
    });

    new Keyboard(document.getElementById('email'), {
      position: 'top center',
      language: 'en-US',
      uppercase: false,
    });

    new Keyboard(document.getElementById('taxpayerIdNumber'), {
      position: 'bottom center',
      numpad: true
    });

    new Keyboard(document.getElementById('idCard'), {
      position: 'bottom center',
      numpad: true
    });

    const datepicker1 = new Datepicker(document.getElementById('idCardIssueDate'), {
      position: 'top center',
      days: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
      months: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентрябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
      startDay: 1,
      dateSelected: new Date(),
      minDate: new Date(1900, 5, 1),
      maxDate: new Date(),
      formatter: (element, date) => {
        // GET YYYY, MM AND DD FROM THE DATE OBJECT
        const yyyy = date.getFullYear().toString();
        const mm = (date.getMonth() + 1).toString();
        const dd  = date.getDate().toString();

        const mmChars = mm.split('');
        const ddChars = dd.split('');
        element.value = yyyy + '/' + (mmChars[1]?mm:"0"+mmChars[0]) + '/' + (ddChars[1]?dd:"0"+ddChars[0]);
      },
    });

    registrationForm.addEventListener('submit', e => {
      e.preventDefault();

      let errors = [];
      const errorList = document.getElementById('errorList');

      const formData = new FormData(registrationForm);

      // 1. Fullname matching: ^[a-zA-ZА-Яа-я ]*$
      let field = formData.get('fullname').match(/^[А-ЯЁа-яёA-Za-z]+(?: [А-ЯЁа-яёA-Za-z]+)*$/);

      if (field === null || field[0].length <= 1) {
        errors.push('Проверьте правильность заполнения поля «ФИО»');
      }

      // 2. Phone matching
      field = formData.get('phone');

      if (field == '' || (field.length > 0 && field.length > 15)) {
        errors.push('Проверьте правильность заполнения поля «Мобильный телефон»');
      }

      // 3. Email matching
      field = formData.get('email');
      if (field.length > 0 || field == '') {
        field = field.match(/\S+@\S+\.\S+/);

        if (field === null) {
          errors.push('Проверьте правильность заполнения поля «Электронная почта»');
        }
      }

      // 4. Taxpayer id matching
      field = formData.get('taxpayerIdNumber');
      if (field.length !== 12) {
        errors.push('Проверьте правильность заполнения поля «ИИН/БИН»');
      }

      // 5. Id card matching
      field = formData.get('idCard');
      if (field.length !== 9) {
        errors.push('Проверьте правильность заполнения поля «№ удостоверения личности»');
      }

      field = formData.get('idCardIssueDate');
      if (field.length === 0) {
        errors.push('Проверьте правильность заполнения поля «Дата получения удостоверения личности»');
      }

      while (errorList.firstChild) {
        errorList.removeChild(errorList.firstChild);
      }

      if (errors.length > 0) {
        errors.forEach((error, index) => {
          const li = document.createElement('LI');
          const textNode = document.createTextNode(`${index + 1}. ${error}`);
          li.appendChild(textNode);
          errorList.appendChild(li);
        });
      } else {
        fetch('https://ipts.agor.kz/api/guest.register', {
          method: 'POST',
          headers: {
            'Authorization': terminalObject.getToken()
          },
          body: formData
        }).then(response => response.json()).then(response => {
          if (response.status == 'success') {
            window.location.href = `/${window.location.pathname.split("/").pop()}`;
          } else {
            const li = document.createElement('LI');
            const textNode = document.createTextNode(`Пользователь с ИИН «${formData.get('taxpayerIdNumber')}» уже существует`);
            li.appendChild(textNode);
            errorList.appendChild(li);
          }
        });
      }
    });
  });
</script>
{@scripts}
