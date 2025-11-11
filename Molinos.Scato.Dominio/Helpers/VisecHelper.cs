using Molinos.Scato.Dominio.Enums;
using System.Text.RegularExpressions;

namespace Molinos.Scato.Dominio.Helpers
{
    public class VisecHelper
    {
        public static TipoOrigenCPE ObtenerTipoOrigenCPE(int plantaOrigen)
        {
            return plantaOrigen == 0 ? TipoOrigenCPE.UnidadProductiva : TipoOrigenCPE.RUCA;
        }

        public static string ObtenerRENSPA(string observacion)
        {
            if (string.IsNullOrEmpty(observacion))
                return string.Empty;

            var matchFormato1 = Regex.Match(observacion, @"\d{2}\.\d{3}\.\d\.\d{5}/\d{2}"); // Formato 11.111.1.11111/11
            if (matchFormato1.Success)
                return matchFormato1.Value;

            var matchFormato2 = Regex.Match(observacion, @"(\d{2})/(\d{9})/(\d{2})"); // Formato 11/111111111/11
            if (matchFormato2.Success)            
                return ConvertirRENSPAFormato2AFormato1(matchFormato2.Value);

            return string.Empty;
        }

        private static string ConvertirRENSPAFormato2AFormato1(string renspaFormato2)
        {
            var partes = renspaFormato2.Split('/');
            if (partes.Length != 3 || partes[1].Length != 9)
                return string.Empty;

            var parte1 = partes[0];           // 11
            var parteCentral = partes[1];     // 111111111
            var parte3 = partes[2];           // 11

            var sub1 = parteCentral.Substring(0, 3); // 111
            var sub2 = parteCentral.Substring(3, 1); // 1
            var sub3 = parteCentral.Substring(4);    // 11111

            return $"{parte1}.{sub1}.{sub2}.{sub3}/{parte3}";
        }
    }
}
