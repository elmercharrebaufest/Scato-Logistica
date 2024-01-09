using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoEstablecimiento : Comando
    {
        public Guid InstanceId { get; set; }
        public int EstablecimientoId { get; set; }
        public int TipoVariedadId { get; set; }
    }
}
