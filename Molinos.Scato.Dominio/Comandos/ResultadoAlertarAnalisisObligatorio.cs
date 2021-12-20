using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ResultadoAlertarAnalisisObligatorio : Resultado
    {
        public ResultadoAlertarAnalisisObligatorio()
        {
            Material = new List<string>();
        }

        public bool AlertarAnalisisObligatorio { get; set; }

        public List<string> Material { get; set; }
    }
}
