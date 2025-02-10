using System;

namespace Molinos.Scato.Dominio.Helpers
{
    public class FormatterHelper
    {
        public static string ConvertirCuilConGuiones(string cuilSinGuiones)
        {
            if (string.IsNullOrEmpty(cuilSinGuiones))
                throw new ArgumentException("El cuil no debe estar vacio");

            if (cuilSinGuiones.Length != 11)
                throw new ArgumentException("El cuil debe tener 11 dígitos");
            
            string validador1 = cuilSinGuiones.Substring(0, 2);
            string documento = cuilSinGuiones.Substring(2, 8);
            string validador2 = cuilSinGuiones.Substring(10, 1);
            return validador1 + "-" + documento + "-" + validador2;
        }

        public static string ConvertirCuilConGuionesSinException(string cuilSinGuiones) 
        {
            if (string.IsNullOrEmpty(cuilSinGuiones))
                return "";

            if (cuilSinGuiones.Length != 11)
                throw new ArgumentException("El cuil debe tener 11 dígitos");

            string validador1 = cuilSinGuiones.Substring(0, 2);
            string documento = cuilSinGuiones.Substring(2, 8);
            string validador2 = cuilSinGuiones.Substring(10, 1);
            return validador1 + "-" + documento + "-" + validador2;
        }

        public static string ObtenerDocumentoDesdeCuilConGuiones(string cuil)
        {
            if (string.IsNullOrEmpty(cuil))
                throw new ArgumentException("El cuil no debe estar vacio");
            
            var identificadores = cuil.Split('-');
            if (identificadores.Length != 3)
                throw new ArgumentException("El cuil debe tener el formato XX-XXXXXXXX-X");

            return identificadores[1];
        }
    }
}
