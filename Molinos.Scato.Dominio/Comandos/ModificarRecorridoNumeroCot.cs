using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoNumeroCot : Comando
    {
        public Guid InstanceId { get; set; }
        public string NumeroCot { get; set; }
    }
}
