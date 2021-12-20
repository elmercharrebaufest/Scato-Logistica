using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RomaneoItem:IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual Romaneo Romaneo { get; set; }
        public virtual Material Material { get; set; }
        public virtual Almacen Almacen { get; set; }
        public virtual string LoteProveedor { get; set; }
        public virtual decimal PesoBruto { get; set; }
        public virtual decimal PesoTara { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual int ItemNro { get; set; }
        public virtual DateTime? FechaFabricacion { get; set; }
        public virtual string DocMaterial { get; set; }
        public virtual string EjercicioDocMaterial { get; set; }
        public virtual string EjercicioDocMaterialAnulacion { get; set; }
        public virtual string RemitoNro { get; set; }
        public virtual bool Rechazado { get; set; }
        public virtual Balanza Balanza { get; set; }
        public virtual TaraRomaneo TaraRomaneo { get; set; }
        public virtual DateTime FechaRemito { get; set; }
    }
}