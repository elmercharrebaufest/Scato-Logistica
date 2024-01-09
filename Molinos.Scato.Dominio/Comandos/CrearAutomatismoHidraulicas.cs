using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearAutomatismoHidraulicas : Comando
    {
        public int IdAutomatismo { get; set; }
        public List<int> Hidraulicas { get; set; }
    }
}