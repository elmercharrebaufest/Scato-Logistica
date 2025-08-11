using System;
using System.Configuration;
using System.IO;

namespace Molinos.Scato.Dominio.Helpers
{
    public static class LoggerHelper
    {
        public static void WriteLine(string message)
        {
            try
            {
                var baseUrl = ConfigurationManager.AppSettings["UrlLogTemporal"];
                if (string.IsNullOrEmpty(baseUrl)) return;
                
                using (StreamWriter outputFile = new StreamWriter(baseUrl, true))
                {
                    outputFile.WriteLine($"{DateTime.Now} - {message}");
                }
            }
            catch
            {
            }
        }
    }
}
