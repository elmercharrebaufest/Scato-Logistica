using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoMuestraAuditoria : Comando
    {
        public Guid InstanceId { get; set; }
    }
}