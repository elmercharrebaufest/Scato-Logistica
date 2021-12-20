using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoPeso : Comando
    {
        public Guid InstanceId { get; set; }
        public TipoPesada TipoPesada { get; set; }
        public int? Peso { get; set; }
        public int? BalanzaId { get; set; }
    }
}
