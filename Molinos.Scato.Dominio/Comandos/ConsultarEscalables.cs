
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarEscalables : Comando
    {
        public string Patente { get; set; }
        public string Acoplado { get; set; }
    }
}