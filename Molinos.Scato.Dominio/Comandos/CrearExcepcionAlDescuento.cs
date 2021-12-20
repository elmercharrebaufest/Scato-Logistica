using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearExcepcionAlDescuento : Comando
    {
        public ExcepcionAlDescuentoDto Dto { get; set; }
    }
}
