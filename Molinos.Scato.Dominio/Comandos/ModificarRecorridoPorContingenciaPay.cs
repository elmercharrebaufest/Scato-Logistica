using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarRecorridoPorContingenciaPay : Comando
    { 
        public Guid InstanceId { get; set; }
    }
}
