using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogIngresoPorPuesto : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        
        public virtual Recorrido Recorrido { get; set; }
        
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        
        public virtual TipoIngresoPorPuesto TipoIngreso { get; set; }
        
        public virtual DateTime FechaHora { get; set; }
    }
}
