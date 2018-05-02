using CefSharp;
using CefSharp.WinForms;
using IPTS.Validator;
using System;

namespace IPTS.Objects
{
    public class Validator
    {
        private BillValidator _billValidator;

        private static ChromiumWebBrowser _chromiumWebBrowser = null;

        public ValidatorObject(ChromiumWebBrowser chromiumWebBrowser, BillValidator billValidator)
        {
            _chromiumWebBrowser = chromiumWebBrowser;

            this._billValidator = billValidator;

            this._billValidator.BillCassetteStatusEvent += _billValidator_BillCassetteStatusEvent;
            this._billValidator.BillReceived += _billValidator_BillReceived;
            this._billValidator.BillStacking += _billValidator_BillStacking;
        }

        private void _billValidator_BillStacking(object Sender, BillStackedEventArgs e)
        {
            //_chromiumWebBrowser.ExecuteScriptAsync(string.Format("console.log('Bill Stacking: {0}');", e.Value));
        }

        private void _billValidator_BillReceived(object Sender, BillReceivedEventArgs e)
        {
            //string script = string.Format("x = y.innerText.replace(/[^0-9\\.]+/g, ''); x = (parseInt(x) + {0}) + '₸'; y.innerText = x;", e.Value);
            string script = string.Format("x = y.innerText.replace(/[^0-9\\.]+/g, ''); x = (parseInt(x) + {0}); y.innerText = x;", e.Value);
            _chromiumWebBrowser.ExecuteScriptAsync(script);
        }

        private void _billValidator_BillCassetteStatusEvent(object Sender, BillCassetteEventArgs e)
        {
            //_chromiumWebBrowser.ExecuteScriptAsync(string.Format("console.log('Bill Cassette Status Event: {0}');", e.Status));
        }

        public void startListening()
        {
            _billValidator.StartListening();
        }

        public void enable()
        {
            _billValidator.EnableBillValidator();
        }

        public void disable()
        {
            _billValidator.DisableBillValidator();
        }

        public void stopListening()
        {
            _billValidator.StopListening();
        }
    }
}
