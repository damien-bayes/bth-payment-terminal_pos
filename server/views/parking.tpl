{@stylesheets}
<style>
</style>
{@stylesheets} {@content}
<section class="main-central-deck" style="position: static;">
  <h2 class='service-title' style="animation: fadeInLeft 1s;"><i class="fa fa-car"></i>Парковка</h2>
  <p style="margin-bottom: .5em;">Данная услуга предназначена для тех, кто путешествует на автомобиле</p>

  <!-- Parking form -->
  <form id='parkingForm' method="post">
    <div class='form-body'>
      <label class='form-label' for='fullname'>Введите фамилию, имя, отчество<i data-wind data-tooltip="Если вы еще не зарегистрированы в системе, пожалуйста пройдите регистрацию пройдя по ссылке «Не проживаете здесь?»" data-position="center top" style="margin-left: .25em;" class="fa fa-question-circle"></i></label>
      <input class='form-control' placeholder='Иванов Иван Иванович' type='text' id='fullname' name='fullname' maxlength="128" />

      <label class='form-label' for="parkingList">Выберите категорию транспортного средства</label>
      <ul class='radio' id="parkingList"></ul>

      <label class='form-label' for='plateNumber'>Введите номер автомобиля</label>
      <input class='form-control' placeholder='864MVA16' type='text' id='plateNumber' name='plateNumber' maxlength="10" />

      <a href='javascript: window.location.href = `/registration${window.location.pathname}`'>Не проживаете здесь?</a>

      <ul class="errorList" id="errorList"></ul>
    </div>

    <div class='form-footer'>
      <button class='button primary-button large-button' id="submit" type="submit">Перейти к оплате</button>
    </div>
  </form>
  <!-- END: Parking form -->

</section>
{@content} {@scripts}
<script src='{@mcrypt="/js/idle.min.js"}'></script>
<script src='{@mcrypt="/js/keyboard.min.js"}'></script>

<script type='text/javascript'>
  document.addEventListener('DOMContentLoaded', _ => {
    // Initialize the module of idling helping users to return to the homepage
    new Idle({
      // Give 2 minutes before returning to the homepage
      delay: 120000,
      onExpire: () => {
        // Return to the homepage on expiring
        window.location.href = '/';
      }
    });

    // Keyboard default values
    const keyboardDefaults = {
      uppercase: false
    };

    // Initialize keyboard instance
    const keyboard1 = new Keyboard(document.getElementById('fullname'), Object.assign({
      // Setting position
      position: 'bottom center',
      // Setting keyboard language
      language: 'ru-RU',
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
    }, keyboardDefaults));

    // Initializing the global click event (Required for the virtual keyboard)
    window.addEventListener('click', e => {
      if (e.type === 'click' && e.path.includes(keyboard1.instance.keyboard)) {
        const value = e.target.nodeName === 'LI' ? e.target.innerText : null;

        if (value) keyboard1.instance.selector.value = value;
      }
    });

    // // Initialize keyboard instance
    new Keyboard(document.getElementById('plateNumber'), Object.assign({
      // Setting position
      position: 'top center',
      // Setting keyboard language
      language: 'en-US'
    }, keyboardDefaults));

    // Get a vehicle element list related to the current page
    const parkingList = document.getElementById('parkingList');

    // Call API for getting parking data depending on hotel settings like price list and etc.
    // ! Get vehicles and price tags related to the parking data
    callApi('hotel.getParking').then(response => {
      // <input class="radio-button" ${index === 0? 'checked' : ''} value="${object.id},${Math.round(object.price)}" type="radio" name="vehicleStation" />
      response.data.forEach((object, index) => {
        const markup =
          `
          <li>
            <input class="radio-button" ${index === 0? 'checked' : ''} value="${object.id}" type="radio" name="vehicleStation" />
            <div class="content">
              <img src="${object.imageUrl}" alt="${object.name}" />
              <label for="vehicleType" class="label">${object.name}</label>
              <span style="font-size: 1.250em;" class="badge">${Math.round(object.price)}₸</span>
            </div>
          </li>
        `;
        parkingList.innerHTML += markup;
      });
    }).catch(error => {
      // Display error data to the browser console
      console.warn(error);
    });

    // Get a form element related to the current page
    const parkingForm = document.getElementById('parkingForm');

    parkingForm.addEventListener('submit', e => {
      e.preventDefault();

      // Disable submit button temporarily
      const submitButton = document.getElementById('submit');
      submitButton.disabled = true;

      // An array storing all types of errors together
      let errors = [];

      // Get an error list element
      const errorList = document.getElementById('errorList');

      // Initialize a new form data instance with appending the current form data
      const formData = new FormData(parkingForm);

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

      // Get a user's car license plate number from the form data
      field = formData.get('plateNumber');

      // Make certain that a car number consists of latin letters and numbers
      //field = field.match(/^[a-zA-Z0-9]+$/);
      field = field.match(/^(?:[0-9]+[a-z]|[a-z]+[0-9])[a-z0-9]*$/);

      // Checking that a car number field is not null and its length is more than 3
      if (field === null || field[0].length <= 3) errors.push('Проверьте правильность заполнения поля «Номер автомобиля»');

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

        // Call API for a parking invoice (hotel.invoiceParking)
        callApi('invoice.parking', formData).then(response => {
          window.location.href = `/payment/receive?service=${response.data.serviceId},${response.data.affectedId}&guestId=${response.data.userId}&expectedMoney=${Math.round(response.data.expectedMoney)}`;
        }).catch(error => {
          const li = document.createElement('li');
          const textNode = document.createTextNode(error);
          li.appendChild(textNode);
          errorList.appendChild(li);

          submitButton.disabled = false;
        });
      }
    });
  });
</script>
{@scripts}
