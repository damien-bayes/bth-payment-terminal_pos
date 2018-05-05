{@stylesheets}
<style>

</style>
{@stylesheets} {@content}
<!-- Reservation -->
<section class="main-central-deck" style="position: static;">
  <h2 class='service-title' style="animation: fadeInLeft 1s;"><i class="fa fa-building-o "></i>Аренда комнат</h2>

  <form id='reservationForm' method='post' style="width: 900px;">
    <div data-section='0'>
      <div class='form-body'>
        <label class='form-label' for='fullName'>Введите фамилию, имя, отчество<i data-wind data-tooltip="Если вы еще не зарегистрированы в системе, пожалуйста пройдите регистрацию пройдя по ссылке «Не проживаете здесь?»" data-position="center top" style="margin-left: .25em;" class="fa fa-question-circle"></i></label>
        <input class='form-control' placeholder='Иванов Иван Иванович' type='text' id='fullname' name='fullname' />

        <label class="form-label">Выберите вариант аренды</label>
        <ul class='radio'>
          <li>
            <input id="shortTerm" class='radio-button' checked type='radio' value="0" name='reservationTerm' />
            <div class='content'>
              <img src='https://ipts.agor.kz/images/wall-clock.svg' style="width: 64px;" />
              <label for='shortTerm' class='label'>Краткосрочная аренда</label>
            </div>
          </li>
          <li>
            <input id="longTerm" class='radio-button' type='radio' value="1" name='reservationTerm' />
            <div class='content'>
              <img src='https://ipts.agor.kz/images/calendar-tool-variant-for-time-administration.svg' style="width: 64px;" />
              <label for='longTerm' class='label'>Долгосрочная аренда</label>
            </div>
          </li>
        </ul>

        <a href='javascript: window.location.href = `/registration${window.location.pathname}`'>Не проживаете здесь?</a>
      </div>
      <div class='form-footer'>
        <button class='button primary-button large-button' data-action='next'>Далее</button>
      </div>
    </div>
    <div data-section='1'>
      <div class='form-body'>
        <label class='form-label' for='taxpayerIdNumber'>Укажите дату заезда и выезда</label>
        <div class='form-row'>
          <div class='form-column'>
            <div class='input-group'>
              <input class='form-control' type='text' placeholder="Пятница, Май 12 2018" id='checkIn' name='checkIn' />
            </div>
          </div>
          <div class='form-column'>
            <div class='input-group'>
              <input class='form-control' type='text' placeholder="Пятница, Май 12 2018" id='checkOut' name='checkOut' />
            </div>
          </div>
        </div>

        <label class='form-label' for='taxpayerIdNumber'>Выберите классификацию комнаты</label>
        <ul class="reservationList" id="reservationList"></ul>

        <!--<p><i class='fa fa-exclamation-triangle'></i> Внимание, исчисление суток с <b>00:00</b> до <b>00:00</b> следующего дня</p>-->
        <p style="margin-bottom: .25em;">К оплате:<span style="margin: 0 .25em;" id='price' class='badge'>0</span>₸</p>

        <a href='javascript: window.location.href = `/registration${window.location.pathname}`'>Не проживаете здесь?</a>
        <ul class="errorList" id="errorList"></ul>
      </div>
      <div class='form-footer'>
        <button class='button large-button' data-action='back'>Назад</button>
        <button class='button primary-button large-button' id="submit">Перейти к оплате</button>
      </div>
    </div>
  </form>
</section>
<!-- END: Reservation -->
{@content} {@scripts}
<script src='{@mcrypt="/js/idle.min.js"}'></script>
<script src='{@mcrypt="/js/keyboard.min.js"}'></script>
<script src='{@mcrypt="/js/rating.js"}'></script>
<script src='{@mcrypt="/js/datepicker.min.js"}'></script>
<script src='{@mcrypt="/js/multiform.js"}'></script>

<script src='{@mcrypt="/js/bayesian/utils.js"}'></script>

<script>
  document.addEventListener('DOMContentLoaded', _ => {
    new Idle({
      delay: 120000, // Give 2 minutes before returning to the homepage
      onExpire: () => {
        window.location.href = '/'; // Return to the homepage on expiring
      }
    });

    const reservationForm = document.getElementById('reservationForm');
    new Multiform(reservationForm).initialize();

    // Initialize keyboard instance
    const keyboard1 = new Keyboard(document.getElementById('fullname'), {
      // Setting position
      position: 'bottom center',
      // Setting keyboard language
      language: 'ru-RU',
      // Is uppercase enabled?
      uppercase: false,
      onChange: (self, instance) => {
        // Retrieving an input value
        const k = instance.selector.value;

        if (k.length > 0) {
          // Initialize a new form data instance
          const formData = new FormData();
          // Append the input value to form data
          formData.append('phrase', k);

          // Call API for getting user names depending on the input value
          callApi('guest.searchByFullname', formData).then(response => {
            // Ensuring that response data is not null or empty
            if (response && response.data.length > 0) {
              clearPrecedingData();

              const searchingList = document.createElement('UL');
              searchingList.classList.add('fullNameSearchingList');

              response.data.forEach(object => {
                const li = document.createElement('LI');
                li.innerHTML = `${object.fullname} <i style='pointer-events: none;'>(${object.taxpayerIdNumber})</i>`;

                searchingList.appendChild(li);
              });
              instance.keyboard.appendChild(searchingList);
            }
          }).catch(error => {
            console.warn(error);
          });
        }
        else { clearPrecedingData(); }

        /**
         * Supporting function for disposing previous data
         */
        function clearPrecedingData() {
          const ul = instance.keyboard.querySelector('UL');
          if (ul !== null) {
            while (ul.firstChild) {
              ul.removeChild(ul.firstChild);
            }
            ul.remove();
          }
        }
      }
    });

    // Initializing the global click event (Required for the virtual keyboard)
    window.addEventListener('click', e => {
      if (e.type === 'click' && e.path.includes(keyboard1.instance.keyboard)) {
        const value = e.target.nodeName === 'LI' ? e.target.innerText : null;

        if (value) keyboard1.instance.selector.value = value;
      }
    });

    let successorDate = new Date();
    successorDate.setDate(successorDate.getDate() + 1);

    let datepicker2 = null;

    const datepickerDefaults = {
      position: 'bottom center',
      days: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
      months: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентрябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
      startDay: 1,
      maxDate: new Date(2118, 0, 1),

      formatter: (element, date) => {
        const fullDays = ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'];

        const format = `${fullDays[date.getDay()]}, ${datepickerDefaults.months[date.getMonth()]} ${date.getDate()} ${date.getFullYear()}`;
        element.value = format;
      }
    };

    const datepicker1 = new Datepicker(document.getElementById('checkIn'), Object.assign({
      minDate: new Date(),
      dateSelected: new Date(),
      onSelect: instance => {
        successorDate.setDate(instance.dateSelected.getDate() + 1);
        successorDate.setYear(instance.dateSelected.getFullYear());

        datepicker2.dispose();

        datepicker2 = new Datepicker(document.getElementById('checkOut'), Object.assign({
          dateSelected: successorDate,
          minDate: successorDate,
          onSelect: () => { calculateTotalPrice(); }
        }, datepickerDefaults));

        calculateTotalPrice();
      }
    }, datepickerDefaults));

    datepicker2 = new Datepicker(document.getElementById('checkOut'), Object.assign({
      dateSelected: successorDate,
      minDate: successorDate,
      onSelect: () => { calculateTotalPrice(); }
    }, datepickerDefaults));

    let termData;
    callApi('hotel.getReservation').then(response => {
      termData = response.data;

      generateTerms(0);
    });

    const reservationTermRadios = document.getElementsByName('reservationTerm');
    reservationTermRadios.forEach(reservationTermRadio => {
      reservationTermRadio.addEventListener('change', e => {
        generateTerms(parseInt(e.target.value));
      });
    });

    function generateTerms(state) {
      const xObj = document.getElementById('reservationList');

      while (xObj.firstChild) xObj.removeChild(xObj.firstChild);

      termData.forEach((object, index) => {
        if (!object.longTermPrice && state === 1) return;

        const markup = `
          <li>
            <input type="radio" name="reservationStation" ${index === 0? 'checked' : ''} value="${object.id},${state === 0? Math.round(object.shortTermPrice) : Math.round(object.longTermPrice)}">
            <div class="reservationCard">
              <div class="reservationCardHeader">
                <h3>${state === 0? Math.round(object.shortTermPrice) : Math.round(object.longTermPrice)}₸<sub>${state === 0? '/в сутки' : '/в месяц'}</sub></h3>
                <p>${object.name}</p>
              </div>
              <div class="reservationCardBody">
                <div class="reservationCardIcon">
                  <img src="${object.imageUrl}"/>
                  <span>x${object.quantity}</span>
                </div>
              </div>
              <div class="reservationCardFooter">
                <p>${object.label}</p>
              </div>
            </div>
          </li>
        `;
        xObj.innerHTML += markup;
      });

      const reservationStationRadios = document.getElementsByName('reservationStation');
      reservationStationRadios.forEach(reservationStationRadio => {
        reservationStationRadio.addEventListener('change', () => {
          calculateTotalPrice();
        });
      });

      calculateTotalPrice();
    }

    function calculateTotalPrice() {
      const k = parseInt(document.querySelector('input[name="reservationTerm"]:checked').value);

      const d1 = datepicker1.instance.dateSelected,
            d2 = datepicker2.instance.dateSelected;

      const diff = k === 0 ? Date.getDayDifference(d1, d2) : Date.getMonthDifference(d1, d2) + 1;

      const n = document.querySelector('input[name="reservationStation"]:checked').value.split(',')[1];

      document.getElementById('price').innerText = n * diff;
    }

    reservationForm.addEventListener('submit', e => {
      e.preventDefault();

      // Disable submit button temporarily
      const submitButton = document.getElementById('submit');
      submitButton.disabled = true;

      // An array storing all types of errors together
      let errors = [];

      // Get an error list element
      const errorList = document.getElementById('errorList');

      // Initialize a new form data instance with appending the current form data
      const formData = new FormData(reservationForm);

      // Initialize a new variable that will store form key's value for validating
      let field = null;

      // Get user's full name from the form data
      field = formData.get('fullname');

      // Extract a taxpayer id number from the full name field
      // Note: As it's known all people can have the same first name and surname but never the same taxpayer id number
      /*
       * Example:
       * John Doe (811030350144) <-
       * John Doe (680403594311)
       */
      const taxpayerIdNumber = field.split('(').filter(function(x) {
          return x.indexOf(')') > -1
      }).map(function(x) {
        return x.split(')')[0]
      });

      // Remove a taxpayer id number and retain a full name only
      field = field.replace(/\s*\(.*?\)\s*/g, '');

      // Remove whitespace from both sides of a string
      field = field.trim();

      // Save a new user's full name
      formData.set('fullname', field);

      // Validate only necessary first name and surname
      /*
       * Example:
       * John Doe
       * Jane Doe Weinstein
       * John
       * Иванов Иван Иванович
       * Иванов Иван
       * Иван
       */
      field = field.match(/^[А-ЯЁа-яёA-Za-z]+(?: [А-ЯЁа-яёA-Za-z]+)*$/);

      // Checking that a full name field is not null and its length is more than 1
      // A good examples are Chinese names such as Po, Hun
      if (field === null || field[0].length === 1) {
        errors.push('Проверьте правильность заполнения поля «ФИО»');
      }

      // Convert date into another format
      formData.set('checkIn', formatDate(datepicker1.instance.dateSelected));
      formData.set('checkOut', formatDate(datepicker2.instance.dateSelected));

      // Remove all previous error elements whenever they were
      while (errorList.firstChild) errorList.removeChild(errorList.firstChild);

      // If a number of errors more than 0, display corresponding errors to a user
      if (errors.length > 0) {
        errors.forEach((error, index) => {
          const li = document.createElement('li');
          const textNode = document.createTextNode(`${index + 1}. ${error}`);
          li.appendChild(textNode);

          errorList.appendChild(li);
        });

        // Enable the submit button
        submitButton.disabled = false;
      }
      else {
        // Appending a user's taxpayer id number to the form data before sending
        formData.append('taxpayerIdNumber', taxpayerIdNumber);
        formData.append('expectedMoney', document.getElementById('price').innerText);

        callApi('invoice.reservation', formData).then(response => {
          window.location.href = `/payment/receive?service=${response.data.serviceId},${response.data.affectedId}&guestId=${response.data.userId}&expectedMoney=${Math.round(response.data.expectedMoney)}`;
        }).catch(error => {
          const li = document.createElement('li');
          const textNode = document.createTextNode(error);
          li.appendChild(textNode);
          errorList.appendChild(li);

          // Enable the submit button
          submitButton.disabled = false;
        });
      }
    });
  });
</script>
{@scripts}
