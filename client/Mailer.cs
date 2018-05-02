using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPTS
{
    public class Smtp
    {
        public string Host { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public int Port { get; private set; }
    }

    public class Mailer
    {
        private static Dictionary<string, string> _smtp = new Dictionary<string, string>
        {
            { "host", ConfigurationManager.AppSettings["SmtpHost"]},
            { "username", ConfigurationManager.AppSettings["SmtpUsername"]},
            { "password", ConfigurationManager.AppSettings["SmtpPassword"]},
            { "port", ConfigurationManager.AppSettings["SmtpPort"]},
            { "from", string.Format("{0} <{1}>", Assembly.GetExecutingAssembly().GetName().Name,  ConfigurationManager.AppSettings["SmtpUsername"])}
        };

        public static bool IsValidEmailAddress(string emailAddress)
        {
            if (!String.IsNullOrEmpty(emailAddress))
            {
                string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

                Regex regex = new Regex(pattern);

                if (regex.IsMatch(emailAddress))
                    return true;
            }

            return false;
        }

        public static void Send(string subject, string body, string[] receivers, Attachment[] attachments)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage(
                    _smtp["from"],
                    string.Join(",", receivers)))
                {
                    mailMessage.Subject = string.Format("{0} - {1}",
                        subject,
                        DateTime.Now.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US")));
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = false;

                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            mailMessage.Attachments.Add(attachment);
                        }
                    }

                    using (SmtpClient smtpClient = new SmtpClient(_smtp["host"], int.Parse(_smtp["port"])))
                    {
                        smtpClient.EnableSsl = true;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(_smtp["username"], _smtp["password"]);
                        smtpClient.Send(mailMessage);
                    }
                }
            }
            catch { }
        }

        public static void Send(string subject, string body, string[] receivers)
        {
            Send(subject, body, receivers, null);
        }
    }
}
