using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearLecturaBalanzada : Comando
    {
        public string CodigoDispositivo { get; set; }

        public Dictionary<string, string> Informacion { get; set; }
    }
}
