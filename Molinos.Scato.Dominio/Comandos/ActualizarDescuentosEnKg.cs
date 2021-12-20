using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarDescuentosEnKg : Comando
    {
        public Guid InstanceId { get; set; }
        public int PesoNeto { get; set; }
    }
}
