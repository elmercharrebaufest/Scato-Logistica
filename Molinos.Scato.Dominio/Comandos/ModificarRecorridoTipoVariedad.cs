using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoTipoVariedad : Comando
    {
        public Guid InstanceId { get; set; }
        public string TipoVariedadCodigo { get; set; }
    }
}