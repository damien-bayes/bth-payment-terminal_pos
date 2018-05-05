<!-- -->
{@stylesheets}{@stylesheets}

{@content}
<div class='container'>
  <div class='row' style='flex: 0 1 100%; display: flex; flex-direction: column; justify-content: center; align-items: center;'>
    <h3>Ooops!</h3>
    <p>Содержимое скоро появится...</p>
  </div>
</div>
{@content}

{@scripts}
<script src='{@mcrypt="/js/idle.js"}'></script>

<script type='text/javascript'>
  document.addEventListener('DOMContentLoaded', _ => {
    let idle = new Idle({delay: 120000}, () => {
      window.location.href = '/';
    }).initialize();
  });
</script>
{@scripts}
