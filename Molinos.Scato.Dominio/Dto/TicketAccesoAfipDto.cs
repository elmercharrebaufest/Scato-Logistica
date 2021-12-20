using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TicketAccesoAfipDto
    {
        public int? Id { get; set; }
        public string Token { get; set; }
        public string Sign { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime GenerationTime { get; set; }
        public string Service { get; set; }
        public string CuitRepresentado { get; set; }  
    }
}