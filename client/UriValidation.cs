using System;

namespace IPTS.Bayesian.Validation
{
    public class UriValidation
    {
        public static bool ValidateUri(string stringUri, out Uri uri)
        {
            Uri resultUri;

            bool isUri = Uri.TryCreate
                (stringUri, UriKind.Absolute, out resultUri)
                &&
                (resultUri.Scheme == Uri.UriSchemeHttp || resultUri.Scheme == Uri.UriSchemeHttps);

            uri = resultUri;

            return isUri;
        }
    }
}
