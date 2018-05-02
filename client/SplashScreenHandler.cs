#region directives
using System;
using System.Threading;
using System.Windows.Forms;
#endregion

namespace IPTS
{
    public static class SplashScreenHandler
    {
        private static SplashScreenForm _splashScreenForm;

        public static void Start()
        {
            Thread splashScreenThread = new Thread(new ThreadStart(StartSplashScreen));
            splashScreenThread.Start();
        }

        private static void StartSplashScreen()
        {
            _splashScreenForm = new SplashScreenForm();
            Application.Run(_splashScreenForm);
        }

        public static void Close()
        {
            if (_splashScreenForm == null) return;

            _splashScreenForm.Invoke(new EventHandler(_splashScreenForm.Kill));
            _splashScreenForm.Dispose();
            _splashScreenForm = null;
        }

        public static void ChangeMessageText(string text)
        {
            if (_splashScreenForm == null) return;

            _splashScreenForm.Invoke(new EventHandler(_splashScreenForm.ChangeMessageText), text);
        }

        public static void BringTop()
        {
            if (_splashScreenForm == null) return;

            _splashScreenForm.Invoke(new EventHandler(_splashScreenForm.BringTop));
        }
    }
}
