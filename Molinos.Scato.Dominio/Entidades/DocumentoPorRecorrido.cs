using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DocumentoPorRecorrido : IIdentificable
    {
        [Key]
        public int Id { get; set; }

        [Column("Recorrido_Id")]
        [ForeignKey(nameof(Recorrido))]
        public int RecorridoId { get; set; }

        public string Path { get; set; }
        public string Extension { get; set; }
        public TipoImpresion Tipo { get; set; }
        public DateTime FechaDeGuardado { get; set; }

        // navegación (NO inicializar)
        public virtual Recorrido Recorrido { get; set; }
    }
}