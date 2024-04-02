using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AutomatismoGrano : IIdentificable
    {
        [Key]
        public int Id { get; set; }

        [Column("Material_Id")]
        public int MaterialId { get; set; }

        [Column("CallePreBalanza_Id")]
        public int CallePreBalanzaId { get; set; }

        public bool AplicaFiltroCalidad { get; set; }

        public bool CamionEscalable { get; set; }

        [Column("Calidad_Id")]
        public int? CalidadId { get; set; }

        public decimal? Minimo { get; set; }

        public decimal? Maximo { get; set; }

        [Column("CallePreHidraulica_Id")]
        public int CallePreHidraulicaId { get; set; }

        [Column("Almacen_Id")]
        public int AlmacenId { get; set; }

        [InverseProperty("AutomatismoGranos")]
        public IList<PuestosDeCargaDescarga> Hidraulicas { get; set; }

        public Calle CallePreBalanza { get; set; }

        public CaracteristicaDeCalidad Calidad { get; set; }

        public Calle CallePreHidraulica { get; set; }

        public Almacen Almacen { get; set; }

        public bool Activo { get; set; }

        public Material Material { get; set; }

        [InverseProperty("AutomatismoGranos")]
        public IList<TipoVariedad> TipoVariedades { get; set; }
    }
}