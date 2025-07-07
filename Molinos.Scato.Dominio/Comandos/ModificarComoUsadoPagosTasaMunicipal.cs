using Molinos.Scato.Dominio.Filtros;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarComoUsadoPagosTasaMunicipal : Comando
    {
        public int PagoId { get; set; }   
        public int DiferenciaPagoId { get; set; }
        public Guid InstanceId { get; set; }
    }
}
