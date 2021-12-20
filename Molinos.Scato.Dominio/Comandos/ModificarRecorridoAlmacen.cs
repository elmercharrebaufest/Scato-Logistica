using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoAlmacen : Comando
    {
        public Guid InstanceId { get; set; }
        public int AlmacenId { get; set; }
    }
}
