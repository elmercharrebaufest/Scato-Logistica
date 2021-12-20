using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("Carga")]
    public class Carga : RegistroBalanzaPuerto, IIdentificable
    {
        [Key, Column(Order = 0)]
        public override int Id { get; set; }
        [Key, Column(Order = 1)]
        public override string NumeroBalanza { get; set; }
        public virtual Vapor Vapor { get; set; }
        public virtual MaterialPuerto Material { get; set; }
        public virtual Bodega Bodega { get; set; } // lo mismo
        public virtual Exportador Exportador { get; set; } // lo mismo
        public virtual Destino Destino { get; set; }
        public virtual int PesoProgramado { get; set; }
        public virtual int ToneladasAW { get; set; } 
        public virtual DateTime? FechaInicio { get; set; }

        [ForeignKey("CargaOpuesta_Id,CargaOpuesta_NumeroBalanza")]
        public virtual Carga CargaOpuesta { get; set; }
        public virtual int? CargaOpuesta_Id { get; set; }

        public virtual string CargaOpuesta_NumeroBalanza { get; set; }

    }
}
