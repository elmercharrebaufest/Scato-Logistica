using System;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConfirmacionCargaDescargaDto
    {
        [DataMember]
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int RecorridoId { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public bool Confirmado { get; set; }
        public bool PendienteConfirmacion { get; set; }
    }
}
