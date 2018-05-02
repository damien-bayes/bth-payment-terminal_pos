#region namespaces
using System;
using CredentialManagement;
using System.Reflection;
using System.Collections.Specialized;
using System.Json;
#endregion

namespace IPTS
{
    public class CredentialManager : ICredentialManager
    {
        private Credential _credential;

        /**
         * Constructor
         */
        public CredentialManager(string target, string description = null)
        {
            _credential = new Credential
            {
                Target = string.IsNullOrEmpty(target) ? Assembly.GetExecutingAssembly().GetName().Name : target,
                Description = description == null ? "Stores important information about an end-user at the specific environment" : description
            };
        }

        /**
         *
         */
        public CredentialData GetCredential()
        {
            if (!_credential.Load())
            {
                // throw new Exception("The credential has not been loaded properly"");
                Logger.Log<CredentialManager>(Logger.Level.Warning, "The credential has not been loaded properly");
            }

            return new CredentialData()
            {
                Username = _credential.Target,
                Password = _credential.Password
            };
        }

        /**
         *
         */
        public void SetCredential(string password, PersistanceType persistanceType)
        {
            _credential.Username = _credential.Target;
            _credential.Password = password;

            _credential.PersistanceType = persistanceType;

            _credential.Save();
        }

        /**
         *
         */
        public Terminal Configure()
        {
            Uri baseAddress = new Uri("https://<YOUR_DOMAIN_NAME>/api");

            // A vault of incoming bytes from the server
            byte[] response = new byte[] { };

            Guid guid = Guid.Empty;

            if (!_credential.Exists())
            {
                guid = Guid.NewGuid();

                SetCredential(guid.ToString(), PersistanceType.LocalComputer);

                response = PostHelpers.PostRequest(string.Format("{0}/terminal.register", baseAddress), new NameValueCollection()
                {
                    {"guid", guid.ToString()},
                    {"username", Environment.UserName},
                    {"machineName", Environment.MachineName},
                    {"operatingSystemVersion", Environment.OSVersion.ToString()}
                });
            }
            else
            {
                CredentialData credentialData = GetCredential();

                guid = Guid.Parse(credentialData.Password);

                response = PostHelpers.PostRequest(string.Format("{0}/terminal.login", baseAddress), new NameValueCollection()
                {
                    {"guid", guid.ToString()}
                });
            }

            string token = GetToken(response);

            return new Terminal() { Token = token };
        }

        /**
         *
         */
        private string GetToken(byte[] byteData)
        {
            if (byteData == null || byteData.Length == 0)
            {
                //throw new Exception("The credential process identifier has not been received from the server");
                Logger.Log<CredentialManager>(Logger.Level.Warning, "The credential process identifier has not been received from the server");

                return null;
            }

            // Encode the byte array to the string (Default: UTF-8)
            string data = System.Text.Encoding.Default.GetString(byteData);

            // Convert JSON to the key value pairs
            JsonValue jsonValue = JsonValue.Parse(data);

            if (jsonValue["status"] != "success")
            {
                //throw new Exception("The credential process has not received relevant status");
                Logger.Log<CredentialManager>(Logger.Level.Warning, "The credential process has not received relevant status");

                return null;
            }

            data = jsonValue["data"].ToString();
            jsonValue = JsonValue.Parse(data);

            return jsonValue["token"];
        }
    }
}
