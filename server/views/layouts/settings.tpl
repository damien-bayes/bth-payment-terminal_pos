<div class='cont'>
  <div class='sidebar-menu' data-component='sidebar-menu' id="sidebarMenu">
    <ul>
      <li>
        <a class='sidebar-menu-item' href="/settings/common">
          <i class='fa fa-fw fa-gears'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Общий
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/validator'>
          <i class='fa fa-fw fa-money'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Купюроприемник
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item' href="/settings/encashment">
          <i class='fa fa-fw fa-usd'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Инкассация
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/printer'>
          <i class='fa fa-fw fa-print'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Принтер
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/notification'>
          <i class='fa fa-fw fa-exclamation'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Оповещения
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/pos'>
          <i class='fa fa-fw fa-desktop'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Терминал
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/update'>
          <i class='fa fa-fw fa-refresh'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Обновления
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/security'>
          <i class='fa fa-fw fa-lock'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Безопасность
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/banner'>
          <i class='fa fa-fw fa-image'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Реклама
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/feedback'>
          <i class='fa fa-fw fa-comments-o'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Отзывы
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/feedback'>
          <i class='fa fa-fw fa-users'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Гости
          </span>
        </a>
      </li>
      <li>
        <a class='sidebar-menu-item disabled' href='/settings/feedback'>
          <i class='fa fa-fw fa-question'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            FAQ
          </span>
        </a>
      </li>
    </ul>
    <ul class='fading'>
      <li>
        <a data-action='fading' class='sidebar-menu-item' href='javascript:' id="exitApp">
          <i class='fa fa-fw fa-angle-left'></i>
          <span data-action='sidebar-menu-text' class='sidebar-menu-text'>
            Выйти
          </span>
        </a>
      </li>
    </ul>
  </div>
  <div class='m' data-content='m'>
    {@content}
  </div>
</div>

<script>
  const params = new URLSearchParams(decodeURIComponent(location.search.slice(1)));

  document.addEventListener('DOMContentLoaded', _ => {
    const sideBarMenu = document.querySelector('#sidebarMenu');
    const hyperlinks = [...sideBarMenu.querySelectorAll('a')];
    const hyperlink = hyperlinks.find(element => element.href === location.href);
    if (hyperlink !== undefined) {
      hyperlink.classList.add('active');
    }

    new Modal(document.getElementById('exitApp'), {
      caption: 'Предупреждение о выходе из системы',
      message: 'Вы уверены что хотите выйти?',
      modalButtonCategory: 5,
      buttonLabels: {
        retry: 'Повторить',
        yes: 'Да',
        no: 'Нет',
        cancel: 'Отмена',
        ok: 'ОК',
        abort: 'Прервать',
        ignore: 'Игнорировать'
      },
      onClickState: (self, instance, status) => {
        if (status === 'no') self.close(instance);
        else if (status === 'yes') {
          console.log('Executing...');
        }
      }
    });
  });
</script>
