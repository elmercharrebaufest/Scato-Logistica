using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.ExternalServices.Repository.Entities.Models
{

    public class TicketAccesoAfipEntity
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string? Token { get; set; }
        public virtual string? Sign { get; set; }
        public virtual DateTime ExpirationTime { get; set; }
        public virtual DateTime GenerationTime { get; set; }
        public virtual string? Service { get; set; }
        public virtual string? CuitRepresentado { get; set; }
        public virtual DateTime? FechaCreacion { get; set; }
    }
}