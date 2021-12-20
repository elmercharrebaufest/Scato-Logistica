
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirResumenDeRecepcion : Comando
    {
        public ImpResumenDeRecepcionDto Dto { get; set; }
        public int CentroId { get; set; }
    }
}
