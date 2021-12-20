using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("Balanzada")]
    public class Balanzada : RegistroBalanzaPuerto, IIdentificable
    {
        [Key, Column(Order = 0)]
        public override int Id { get; set; }
        [Key, Column(Order = 1)]
        public override string NumeroBalanza { get; set; }
        public virtual int PesoBruto { get; set; }
        public virtual int PesoTara { get; set; }
        public virtual int PesoNeto { get; set; }
        public virtual string Capacidad { get; set; }
        [ForeignKey("CargaInicial_Id,CargaInicial_NumeroBalanza")]
        public virtual Carga CargaInicial { get; set; }
        public virtual int CargaInicial_Id { get; set; }

        public virtual string CargaInicial_NumeroBalanza { get; set; }
        public string ErrorSap { get; set; }

        public virtual ModuloDeCargaBalanzas ModuloDeCargaBalanzas { get; set; }

    }
}
