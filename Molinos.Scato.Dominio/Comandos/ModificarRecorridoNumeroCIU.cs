using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoNumeroCIU : Comando
    {
        public Guid InstanceId { get; set; }
        public string Numero { get; set; }
    }
}
