using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearHuellaDigital : Comando
    {
        public HuellaDigitalDto Dto { get; set; }
    }
}
