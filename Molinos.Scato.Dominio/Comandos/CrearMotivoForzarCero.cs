using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearMotivoForzarCero : Comando
    {
        public int BalanzaId { get; set; }
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; }
        public Guid InstanceId { get; set; }

    }
}
