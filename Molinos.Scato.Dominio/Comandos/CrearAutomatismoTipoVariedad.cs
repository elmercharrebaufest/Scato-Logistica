using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearAutomatismoTipoVariedad : Comando
    {
        public int IdAutomatismo { get; set; }
        public List<int> TipoVariedades { get; set; }
    }
}