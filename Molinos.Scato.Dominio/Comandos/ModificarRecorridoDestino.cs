using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoDestino : Comando
    {
        public Guid InstanceId { get; set; }
        public int AlmacenId { get; set; }
        public int? HidraulicaId { get; set; }
        public int? CalleId { get; set; }
        public int? ProximaBalanzaId { get; set; }
        public TipoPesada TipoPesada { get; set; }
        public string UsuarioBruto { get; set; }
        public string UsuarioTara { get; set; }
    }
}
