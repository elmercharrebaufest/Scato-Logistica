using System;
using System.IO;

namespace Molinos.Scato.Dominio.Helpers
{
    public static class LoggerHelper
    {
        public static void WriteLine(string message)
        {
            using (StreamWriter outputFile = new StreamWriter(@"C:\ScatoLogs\logTemporal.txt", true))
            {
                outputFile.WriteLine($"{DateTime.Now} - {message}");
            }
        }
    }
}
