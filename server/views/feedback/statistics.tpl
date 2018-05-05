<!-- -->
{@stylesheets}{@stylesheets}

{@content}
<section class="main-central-deck" style="position: static;">
    <h3>Ooops!</h3>
    <p>Содержимое скоро появится...</p>
</section>
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
