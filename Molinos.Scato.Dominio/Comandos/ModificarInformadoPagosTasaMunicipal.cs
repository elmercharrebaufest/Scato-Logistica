using Molinos.Scato.Dominio.Filtros;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarInformadoPagosTasaMunicipal : Comando
    {
        public Guid InstanceId { get; set; }
    }
}
