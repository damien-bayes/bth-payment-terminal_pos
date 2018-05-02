#region namespaces
using IPTS.Bayesian.Validation;
using System;
using System.Collections.Specialized;
#endregion

namespace IPTS
{
    public static class PostHelpers
    {
        /**
         *
         */
        public static byte[] PostRequest(Uri uri, NameValueCollection nameValueCollection)
        {
            return Http.Post(uri.OriginalString, nameValueCollection);
        }

        /**
          *
          */
        public static byte[] PostRequest(string stringUri, NameValueCollection nameValueCollection)
        {
            Uri uri;
            if (!UriValidation.ValidateUri(stringUri, out uri))
                return null;

            return PostRequest(uri, nameValueCollection);
        }
    }
}
