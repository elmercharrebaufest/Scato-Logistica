using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CaladoEnPlantaPorCaracteristica : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual CaracteristicaDeCalidad CaracteristicaDeCalidad { get; set; }
        public virtual CaladoEnPlanta CaladoEnPlanta { get; set; }
        public virtual decimal? ValorCalado { get; set; }
        public virtual decimal? ValorCaladoEnPlanta { get; set; }
        public virtual string Unidad { get; set; }
        public virtual string Rango { get; set; }
    }
}
