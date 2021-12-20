using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaMangueraCarga : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual DateTime? FechaConexionMangueras { get; set; }
        public virtual string HoraConexionMangueras { get; set; }
        public virtual DateTime? FechaDesconexionMangueras { get; set; }
        public virtual string HoraDesconexionMangueras { get; set; }
        public virtual DateTime? FechaComienzoCarga { get; set; }
        public virtual string HoraComienzoCarga { get; set; }
        public virtual DateTime? FechaFinalizacionCarga { get; set; }
        public virtual string HoraFinalizacionCarga { get; set; }
    }
}