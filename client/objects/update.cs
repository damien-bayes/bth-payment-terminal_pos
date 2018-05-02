using CefSharp;
using CefSharp.WinForms;
using IPTS.Update;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace IPTS.Objects
{
    public class UpdateObject
    {
        private Uri _versionInformationUri = new Uri("https://<YOUR_DOMAIN_NAME>.com/update/version.txt");

        private Uri _updateLink;
        private string _hash = string.Empty;

        private static ChromiumWebBrowser _chromiumWebBrowser = null;

        #region Properties
        public bool UpdateIsAvailable {
            get
            {
                return CheckForUpdate();
            }
        }

        public string CurrentVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        #endregion

        public UpdateObject(ChromiumWebBrowser chromiumWebBrowser)
        {
            _chromiumWebBrowser = chromiumWebBrowser;


            CheckForUpdate();
        }

        /// <summary>
        /// Uri validation
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        private bool UriValidation(string uriString, out Uri uri)
        {
            Uri resultUri;
            bool isUri = Uri.TryCreate
                (uriString, UriKind.Absolute, out resultUri)
                &&
                (resultUri.Scheme == Uri.UriSchemeHttp || resultUri.Scheme == Uri.UriSchemeHttps);

            uri = resultUri;
            return isUri;
        }

        /// <summary>
        /// Checking for update
        /// </summary>
        public bool CheckForUpdate()
        {
            Stream stream = Stream.Null;
            string data = string.Empty;

            using (WebClient client = new WebClient())
            {
                try { stream = client.OpenRead(this._versionInformationUri); }
                catch (Exception exception)
                {
                    //throw new Exception(exception.Message);
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    data = reader.ReadLine() ?? "";
                }
            }

            string[] division = data.Split(';');
            if (division.Length != 3) { return false; }

            if (!UriValidation(division[1], out this._updateLink)) { return false; }

            Version updateVersion = new Version(division[0]);
            string hash = division[2];

            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            if (updateVersion > currentVersion)
            {
                return true;
            }

            return false;
        }

        public void StartUpdating()
        {
            if (this.UpdateIsAvailable)
            {
                string tempPath = Path.GetTempPath();
                string[] segments = _updateLink.Segments;
                string fileName = segments[segments.Length - 1];
                var fullPath = tempPath + fileName;

                if (File.Exists(fullPath)) { File.Delete(fullPath); }

                Thread thread = new Thread(() =>
                {
                    WebClient client = new WebClient();
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                    client.DownloadFileAsync(_updateLink, fullPath);
                });
                thread.Start();
            }
        }

        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string script = string.Format("document.getElementById('message').innerText='Update has been downloaded';");
            _chromiumWebBrowser.ExecuteScriptAsync(script);

            string tempPath = Path.GetTempPath();
            string[] segments = _updateLink.Segments;
            string fileName = segments[segments.Length - 1];
            var fullPath = tempPath + fileName;
            Process.Start(fullPath);

            /*Uri baseAddress = new Uri("https://<YOUR_DOMAIN_NAME>/api");
            Utils.PostRequest(string.Format("{0}/terminal.update", baseAddress), new NameValueCollection()
            {
                {"guid"}
            });*/

            try
            {
                Application.Exit();
            }
            catch(Exception exception)
            {
                Logger.Log<UpdateObject>(exception);
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;

            string script1 = string.Format("document.getElementById('percentage').innerText='{0}%';", int.Parse(Math.Truncate(percentage).ToString()));
            string script2 = string.Format("document.getElementById('message').innerText='Downloaded {0} of {1}';", e.BytesReceived, e.TotalBytesToReceive);

            _chromiumWebBrowser.ExecuteScriptAsync(string.Concat(script1, script2));
        }
    }
}
