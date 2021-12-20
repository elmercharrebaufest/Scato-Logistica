using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoEstablecimientoDemorado : Comando
    {
        public Guid WorkflowInstanceId { get; set; }
    }
}