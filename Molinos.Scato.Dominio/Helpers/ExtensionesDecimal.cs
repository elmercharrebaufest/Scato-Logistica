using System;
using System.Globalization;

namespace Molinos.Scato.Dominio.Helpers
{
    public static class ExtensionesDecimal
    {
        /// <summary>
        /// Parsea un string a decimal intentando con diferentes culturas (punto y coma como separador decimal)
        /// </summary>
        /// <param name="valor">String a parsear</param>
        /// <returns>Valor decimal parseado</returns>
        /// <exception cref="FormatException">Si no se puede parsear el valor</exception>
        public static decimal ParsearDecimal(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("El valor no puede estar vacío");

            valor = valor.Trim();

            if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal resultado))
                return resultado;

            if (decimal.TryParse(valor, NumberStyles.Number, new CultureInfo("es-AR"), out resultado))
                return resultado;

            valor = valor.Replace(',', '.');
            if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out resultado))
                return resultado;

            throw new FormatException($"No se puede convertir '{valor}' a decimal");
        }

        /// <summary>
        /// Intenta parsear un string a decimal intentando con diferentes culturas (punto y coma como separador decimal)
        /// </summary>
        /// <param name="valor">String a parsear</param>
        /// <param name="resultado">Valor decimal parseado si tiene éxito</param>
        /// <returns>True si se pudo parsear, False en caso contrario</returns>
        public static bool TryParsearDecimal(this string valor, out decimal resultado)
        {
            resultado = 0;

            if (string.IsNullOrWhiteSpace(valor))
                return false;

            valor = valor.Trim();

            if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out resultado))
                return true;

            if (decimal.TryParse(valor, NumberStyles.Number, new CultureInfo("es-AR"), out resultado))
                return true;

            valor = valor.Replace(',', '.');
            if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out resultado))
                return true;

            return false;
        }

        /// <summary>
        /// Convierte un string a decimal nullable intentando con diferentes culturas
        /// </summary>
        /// <param name="valor">String a parsear</param>
        /// <returns>Valor decimal nullable</returns>
        public static decimal? ToDecimalNullable(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            if (TryParsearDecimal(valor, out decimal resultado))
                return resultado;

            return null;
        }
    }
}
