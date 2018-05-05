// References: https://github.com/quozd/awesome-dotnet#api

#region Namespaces
using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Reflection;
using System.IO;
using System.Net;
using System.Text;
using IPTS.Objects;
using CredentialManagement;
using System.Collections.Specialized;
using System.Json;
using IPTS.Validator;
using IPTS.Handlers;
using System.Drawing;
using IPTS.Bayesian;
#endregion

namespace IPTS
{
    public partial class BrowserForm : Form
    {
        internal string _defaultUrlAddress = "https://<DOMAIN_NAME>.com";

        internal string _defaultLogFileName = "DefaultLogs.log";

        private static string _defaultIniPath = string.Format("{0}\\config.ini", Directory.GetCurrentDirectory());
        private static Uri _baseAddress;

        private ChromiumWebBrowser _chromiumWebBrowser;
        private BillValidator _billValidator;

        private Terminal _terminal;
        private CredentialManager _credentialManager;

        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;

        //public TelegramManager _telegramManager;

        /// <summary>
        /// Main form
        /// </summary>
        public BrowserForm()
        {
            InitializeComponent();

            Text = Assembly.GetExecutingAssembly().GetName().Name;

            this.FormClosing += BrowserForm_FormClosing;

            // A system tray initialization
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Exit", new EventHandler(delegate (Object obj, EventArgs e)
            {
                this.Close();
            }));
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = $"{Text} {Assembly.GetExecutingAssembly().GetName().Version}";
            _trayIcon.Icon = Utils.GetIconFromUrl($"{_defaultUrlAddress}/images/brand48x48.png");
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;

            // Adding the handler to show log messages (ILoggerHandler)
            Logger.LoggerHandlerManager.AddHandler(new FileLoggerHandler(_defaultLogFileName, string.Format("{0}\\logs", Directory.GetCurrentDirectory())));
            Logger.Log(Logger.Level.Info, string.Format("{0} has been started at {1}", Text, DateTime.Now.ToString("dddd, MMMM dd, yyyy h:mm:ss tt")));

            if (!File.Exists(_defaultIniPath))
            {
                Logger.Log<BrowserForm>(Logger.Level.Warning, ".ini file being used doesn't exist!");
            }
            _baseAddress = new Uri(INI.ReadFile(_defaultIniPath).Get("App", "Source"));

            // Checking for a bill validator existence
            _billValidator = new BillValidator(
                INI.ReadFile(_defaultIniPath).Get("BillAcceptor", "PortName"),
                Int32.Parse(INI.ReadFile(_defaultIniPath).Get("BillAcceptor", "BaudRate"))
            );
            _billValidator.BillReceived += _billValidator_BillReceived;
            _billValidator.BillStacking += _billValidator_BillStacking;
            _billValidator.BillCassetteStatusEvent += _billValidator_BillCassetteStatusEvent;

            _billValidator.PowerUp();

            _credentialManager = new CredentialManager(null);
            _terminal = _credentialManager.Configure();

            // Initializes Chromium (3.2883.1552)
            InitializeChromiumWebBrowser();

            //OldUpdateManager.Start();
            //InitializeTelegram();
            //Mailer.Send("IPTS", _baseAddress.OriginalString, new string[] { "you@example.com" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void _billValidator_BillCassetteStatusEvent(object Sender, BillCassetteEventArgs e)
        {
            Logger.Log(Logger.Level.Info, string.Format("Bill validator status event: {0}", e.Status));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void _billValidator_BillStacking(object Sender, BillStackedEventArgs e)
        {
            Logger.Log(Logger.Level.Info, string.Format("Banknote stacking. Sum: {0}₸", e.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void _billValidator_BillReceived(object Sender, BillReceivedEventArgs e)
        {
            Logger.Log(Logger.Level.Info, string.Format("Banknote rejecting. Reason: {0}. Status: {1}. Sum: {2}₸", e.RejectedReason, e.Status, e.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeChromiumWebBrowser()
        {
            // Start the browser after initializing a global component
            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.WebSecurity = CefState.Disabled;
            // Allow the use of local resources in the browser
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            browserSettings.DefaultEncoding = "UTF-8";
                
            _chromiumWebBrowser = new ChromiumWebBrowser(_baseAddress.OriginalString)
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(_chromiumWebBrowser);

            _chromiumWebBrowser.BrowserSettings = browserSettings;

            // CefSharp Javascript objects (cef, terminal, validator, printer?, telegram?)
            _chromiumWebBrowser.RegisterJsObject("updateObject", new UpdateObject(_chromiumWebBrowser));
            _chromiumWebBrowser.RegisterJsObject("customObject", new CustomObject(_chromiumWebBrowser, this));
            _chromiumWebBrowser.RegisterJsObject("terminalObject", new TerminalObject(_terminal));
            _chromiumWebBrowser.RegisterJsObject("validatorObject", new ValidatorObject(_chromiumWebBrowser, _billValidator));
            _chromiumWebBrowser.RegisterJsObject("printerObject", new PrinterObject(INI.ReadFile(_defaultIniPath).Get("Printer", "Name")));

            _chromiumWebBrowser.RegisterJsObject("credentialManager", _credentialManager);

            _chromiumWebBrowser.LoadError += _chromiumWebBrowser_LoadError;
        }

        /**
         * 
         */
        private void _chromiumWebBrowser_LoadError(object sender, LoadErrorEventArgs e)
        {
            DisplayErrorPage(e.ErrorCode.ToString());
        }

        private void DisplayErrorPage(string message)
        {
            string htmlDirectory = string.Format("{0}\\error.html", Directory.GetCurrentDirectory());
            var html = File.ReadAllText(htmlDirectory);

            html = html.Replace("{@errorMessage}", message);

            _chromiumWebBrowser.LoadString(html, htmlDirectory);
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeTelegram()
        {
            //var accessToken = ConfigurationManager.AppSettings["TelegramAccessToken"];  
            //var t = Task.Run(() => RunTelegram(accessToken));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        public void RunTelegram(string accessToken)
        {
            /*
            _telegramManager = new TelegramManager(accessToken);
            
            if (_telegramManager.MakeAsyncRequest(new GetMe()).Result != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\U00002757 <b>Message</b>\n");
                
                sb.Append(string.Format("App Version: {0} ({1})\n", Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now.ToString("ddMMyyyyHHmm")));
                sb.Append(string.Format("Build: {0}\n", Environment.Is64BitProcess ? "x64" : "x86"));
                sb.Append(string.Format("Date: {0}\n", DateTime.Now.ToString("dddd, MMMM d, yyyy")));
                sb.Append(string.Format("OS Version: {0}\n", Environment.OSVersion));
                sb.Append(string.Format("PC: {0}\n", Environment.MachineName));
                sb.Append(string.Format("Username: {0}\n", Environment.UserName));
                sb.Append(string.Format("IP-Address: {0}\n\n", Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString()));

                var message = _telegramManager.MakeAsyncRequest(new SendMessage(int.Parse(ConfigurationManager.AppSettings["TelegramGroupId"]), sb.ToString())).Result;
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _chromiumWebBrowser.Dispose();
                Cef.Shutdown();

                Logger.Log(Logger.Level.Info, string.Format("{0} app has been closed at {1}", Assembly.GetExecutingAssembly().GetName().Name, DateTime.Now));

                Dispose();
            }
            catch (Exception exception)
            {
                Logger.Log<BrowserForm>(exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserForm_Load(object sender, EventArgs e)
        {
            SplashScreenHandler.Close();
        }
    }
}
