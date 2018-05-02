using CefSharp.WinForms;
using System;
using System.Diagnostics;
using CefSharp;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net;

namespace IPTS.Objects
{
    public class CustomObject
    {
        private static ChromiumWebBrowser _chromiumWebBrowser = null;

        private static BrowserForm _browserForm = null;

        public CustomObject(ChromiumWebBrowser chromiumWebBrowser, BrowserForm browserForm)
        {
            _chromiumWebBrowser = chromiumWebBrowser;
            _browserForm = browserForm;
        }

        public void loadDefaultUri(string uriString = "https://<YOUR_DOMAIN_NAME>.com")
        {
            Uri resultUri;
            bool isUri = Uri.TryCreate
                (uriString, UriKind.Absolute, out resultUri)
                &&
                (resultUri.Scheme == Uri.UriSchemeHttp || resultUri.Scheme == Uri.UriSchemeHttps);

            if (isUri)
            {
                _chromiumWebBrowser.Load(resultUri.OriginalString);
            }
        }

        /**
         *
         */
        public bool checkInternetConnection()
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead("https://www.google.com"))
                        return true;
                }
            }
            catch { return false; }
        }

        public void showMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void showDevTools()
        {
            _chromiumWebBrowser.ShowDevTools();
        }

        public void exitFromApplication()
        {
            Application.Exit();
        }

        public void openCommandPrompt()
        {
            ProcessStartInfo start = new ProcessStartInfo("cmd.exe", "/c pause");
            Process.Start(start);
        }

        public void TestCallback(IJavascriptCallback javascriptCallback)
        {
            const int taskDelay = 1500;

            Task.Run(async () =>
            {
                await Task.Delay(taskDelay);

                using (javascriptCallback)
                {
                    //NOTE: Classes are not supported, simple structs are
                    //var response = new CallbackResponseStruct("Hello World");
                    //await javascriptCallback.ExecuteAsync(response);
                }
            });
        }
    }
}
