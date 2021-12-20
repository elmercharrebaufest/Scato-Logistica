using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EliminarCasilleros : Comando
    {
        public IList<int> Id { get; set; }
    }
}
