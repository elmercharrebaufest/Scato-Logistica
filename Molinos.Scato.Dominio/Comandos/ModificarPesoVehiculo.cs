
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarPesoVehiculo : Comando
    {
        public Guid Id { get; set; }
        public int PesoBruto { get; set; }
        public int PesoTara { get; set; }

    }
}
