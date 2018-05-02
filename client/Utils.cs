using IPTS.Bayesian.Validation;
using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace IPTS.Bayesian
{
    public class Utils
    {
        public static Icon GetIconFromUrl(string uriString, int width = 32, int height = 32)
        {
            Uri uri;
            if (UriValidation.ValidateUri(uriString, out uri))
            {
                return GetIconFromUrl(uri, new Size(width, height));
            }

            return null;
        }

        public static Icon GetIconFromUrl(Uri uri, Size size)
        {
            WebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            MemoryStream memoryStream;

            using (Stream response = request.GetResponse().GetResponseStream())
            {
                memoryStream = new MemoryStream();
                byte[] buffer = new byte[1024];
                int byteCount;

                do
                {
                    byteCount = response.Read(buffer, 0, buffer.Length);
                    memoryStream.Write(buffer, 0, byteCount);
                } while (byteCount > 0);
            }

            bitmap = new Bitmap(Image.FromStream(memoryStream));

            if (bitmap != null) return Icon.FromHandle(bitmap.GetHicon());

            return null;
        }
    }
}
