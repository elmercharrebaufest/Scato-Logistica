
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirPrueba : Comando
    {
        public string NombreUsuario { get; set; }
        public string NombreServidor { get; set; }
        public string NombreImpresora { get; set; }
        public int CantidadCopias { get; set; }
        public bool IsZebra { get; set; }
    }
}
