var telegram = require('telegram-bot-api');
var util = require('util');
var fetch = require('node-fetch');

var FormData = require('form-data');
var fs = require('fs');

var api = new telegram({
  token: '',
  updates: {
    enabled: true
  }
});

api.on('message', function(message) {
  let separated = null,
    context = '';

  if (message.text.includes('/location', '/getallfeedbackbyid')) {
    separated = message.text.split(' ');

    message.text = separated[0];
    separated[1] = separated.length !== 2 ? -1 : separated[1];
  }

  switch (message.text) {
    case '/help':
      const commandList = {
        hotels: [{
          command: '/getallhotels',
          description: 'Возращает наименование, адрес и идентификатор всех отелей зарегистрированных в системе\n\n'
        }],
        feedback: [{
            command: '/getallfeedback',
            description: 'Извлечь все оставленные отзывы со всех терминалов\n'
          },
          {
            command: '/getfeedbackcount',
            description: 'Востребовать общее количество отзывов со всех терминалов\n'
          },
          {
            command: '/getallfeedbackbyid',
            description: 'Получить все оставленные отзывы с определенного терминала используя его идентификатор. _Пример: /getallfeedbackbyid <ID>_\n'
          },
          {
            command: '/getfeedbackcountbyid',
            description: 'Извлечь общее количество отзывов с идентифицированного терминала. _Пример: /getfeedbackcountbyid <ID>_\n\n'
          }
        ],
        miscellaneous: [{
            command: '/time',
            description: 'Вывести текущую дату и время\n'
          },
          {
            command: '/location',
            description: 'Узнать местоположение известных системе терминалов по его идентификатору. _Пример: /location <ID>_\n'
          },
          {
            command: '/help',
            description: 'Получить вспомогательную информацию о всех командах определяемых архитектурой системы\n\n'
          }
        ]
      }

      for (let i = 0; i < Object.keys(commandList).length; i++) {
        commandList[Object.keys(commandList)[i]].forEach(element => {
          context += `${element.command} - ${element.description}`;
        });
      }

      api.sendMessage({
          chat_id: message.chat.id,
          text: '*Уважаемые клиенты!*\n\nВас приветствует автоматический сервис *«IRC IPTS Service»* с помощью которого вы сможете отправлять следующие команды и получать нужную вам инфомацию:\n\n' + context + '',
          parse_mode: 'Markdown'
        })
        .then(function(data) {
          console.log(util.inspect(data, false, null));
        })
        .catch(function(err) {
          console.log(err);
        });
      break;

    case '/getallhotels':
      fetch('https://<DOMAIN_NAME>.com/api/hotel.getAll').then(function(response) {
        return response.json();
      }).then(function(response) {
        response.data.forEach(element => {
          context += `*Идентификатор:* ${element.id}\n*Адрес:* ${element.address}\n*Наименование:* ${element.hotelName}\n\n`;
        })

        api.sendMessage({
            chat_id: message.chat.id,
            text: context,
            parse_mode: 'Markdown'
          })
          .then(function(data) {
            console.log(util.inspect(data, false, null));
          })
          .catch(function(err) {
            console.log(err);
          });
      });
      break;

    case '/getfeedbackcount':
      fetch('https://<DOMAIN_NAME>.com/api/feedback.getCount').then(function(response) {
        return response.json();
      }).then(function(response) {
        api.sendMessage({
            chat_id: message.chat.id,
            text: `*Общее количество отзывов со всех терминалов:* ${response.data[0].count}\n\n`,
            parse_mode: 'Markdown'
          })
          .then(function(data) {
            console.log(util.inspect(data, false, null));
          })
          .catch(function(err) {
            console.log(err);
          });
      });
      break;

      /**
       * Location
       */
    case '/location':
      let detector = false;

      const whereaboutsData = [{
          id: 48,
          address: '',
          institution: '',
          longitude: 0,
          latitude: 0
        },
        {
          id: 134,
          address: '',
          institution: 'Гостиница',
          longitude: 0,
          latitude: 0
        }
      ];

      whereaboutsData.forEach(element => {
        if (element.id == separated[1]) {
          api.sendLocation({
              chat_id: message.chat.id,
              longitude: element.latitude,
              latitude: element.longitude
            })
            .then(function(data) {
              console.log(util.inspect(data, false, null));
            })
            .catch(function(err) {
              console.log(err);
            });
          api.sendMessage({
              chat_id: message.chat.id,
              text: `${element.address}, ${element.institution}`
            })
            .then(function(data) {
              console.log(util.inspect(data, false, null));
            })
            .catch(function(err) {
              console.log(err);
            });

          detector = true;

          return;
        }
      });

      if (!detector) {
        api.sendMessage({
            chat_id: message.chat.id,
            text: `Местоположение терминала по идентификатору «${separated[1]}» не найдено.`
          })
          .then(function(data) {
            console.log(util.inspect(data, false, null));
          })
          .catch(function(err) {
            console.log(err);
          });
      }
      break;

    case '/getallfeedbackbyid':
      let formData = new FormData();
      formData.append('terminalGuid', separated[1]);

      fetch('https://<DOMAIN_NAME>.com/api/feedback.getAllById', {
        method: 'post',
        body: formData
      }).then(function(response) {
        return response.json();
      }).then(function(response) {
        console.log(response);

        response.data.forEach((element, index) => {
          context += `${index + 1}. *Дата создания:* ${element.created}; *Рейтинг:* ${element.rating}; *Сообщение:* ${element.message}; *Статус подтверждения:* ${element.approved}\n\n`;
        })

        api.sendMessage({
            chat_id: message.chat.id,
            text: `*Список оставленных отзывов с терминала ${separated[1]}:*\n\n${context}`,
            parse_mode: 'Markdown'
          })
          .then(function(data) {
            console.log(util.inspect(data, false, null));
          })
          .catch(function(err) {
            console.log(err);
          });
      });
      break;

      /**
       * Time
       */
    case '/time':
      api.sendMessage({
          chat_id: message.chat.id,
          text: `*Текущая дата и время:* ${new Date().toLocaleString()}`,
          parse_mode: 'Markdown'
        })
        .then(function(data) {
          console.log(util.inspect(data, false, null));
        })
        .catch(function(err) {
          console.log(err);
        });
      break;

    case '/getallfeedback':
      fetch('https://<DOMAIN_NAME>.com/api/feedback.getAll').then(function(response) {
        return response.json();
      }).then(function(response) {
        response.data.forEach((element, index) => {
          context += `${index + 1}. *Дата создания:* ${element.created}; *Рейтинг:* ${element.rating}; *Сообщение:* ${element.message}; *Статус подтверждения:* ${element.approved}\n\n`;
        })

        api.sendMessage({
            chat_id: message.chat.id,
            text: `*Список оставленных отзывов со всех терминалов:*\n\n${context}`,
            parse_mode: 'Markdown'
          })
          .then(function(data) {
            console.log(util.inspect(data, false, null));
          })
          .catch(function(err) {
            console.log(err);
          });
      });
      break;
  }
});
