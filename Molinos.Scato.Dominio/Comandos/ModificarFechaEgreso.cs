using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarFechaEgreso : Comando
    {
        public DateTime Fecha { get; set; }
        public Guid WorkflowInstanciaId { get; set; }
    }
}
