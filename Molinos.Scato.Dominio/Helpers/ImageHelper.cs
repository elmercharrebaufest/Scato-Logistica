using System;
using System.IO;

namespace Molinos.Scato.Dominio.Helpers
{
    public static class ImageHelper
    {
        public static string ConvertirUrlABase64(string url)
        {
            if (string.IsNullOrEmpty(url) || !File.Exists(url))
                return null;

            var bytes = File.ReadAllBytes(url);
            return Convert.ToBase64String(bytes);
        }
    }
}
