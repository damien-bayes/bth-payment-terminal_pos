#region namespaces
using System;
using System.Collections.Specialized;
using System.Net;
#endregion

namespace IPTS
{
    public static class Http
    {
        public static byte[] Post(string url, NameValueCollection pairs)
        {
            byte[] response = null;

            try
            {
                using (WebClient client = new WebClient())
                {
                    response = client.UploadValues(url, pairs);
                }
            }
            catch (Exception) { }

            return response;
        }
    }
}
