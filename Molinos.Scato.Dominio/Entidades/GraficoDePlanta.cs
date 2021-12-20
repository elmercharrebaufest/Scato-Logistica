

using Molinos.Scato.Dominio.Enums;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class GraficoDePlanta : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual string NombreActividad { get; set; }
        public virtual int CantidadCamionesNoDemorados { get; set; }
        public virtual int CantidadCamionesDemorados { get; set; }

        public virtual string Color { get; set; }

        public virtual int Rango { get; set; }

        public virtual SectorEnum Sector {get; set;}
    }
}
