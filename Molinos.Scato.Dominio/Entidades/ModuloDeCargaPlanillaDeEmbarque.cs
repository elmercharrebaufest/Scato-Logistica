using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaPlanillaDeEmbarque : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual Exportador Exportador { get; set; }
        public virtual int BodegaParcel { get; set; }
        public virtual string TanqueDeAbordo { get; set; }
        public virtual Destino Destino { get; set; }
        public virtual string Tk { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
        public virtual DateTime? FechaComienzoCarga { get; set; }
        public virtual string HoraComienzoCarga { get; set; }
        public virtual DateTime? FechaFinalizacionCarga { get; set; }
        public virtual string HoraFinalizacionCarga { get; set; }
    }
}