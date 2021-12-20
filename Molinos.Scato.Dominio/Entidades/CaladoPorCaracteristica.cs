using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CaladoPorCaracteristica : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual CaracteristicaDeCalidad CaracteristicaDeCalidad { get; set; }
        public virtual Calado Calado { get; set; }
        public virtual bool AnalisisPreliminar { get; set; }
        public virtual bool HuboExcepcion { get; set; }
        public virtual decimal? ValorCalado { get; set; }
        public virtual decimal? DescuentoEnKg { get; set; }
        public virtual decimal? DescuentoEnPorcentaje { get; set; }
        public virtual string Unidad { get; set; }
        public virtual string Rango { get; set; }
        public virtual ICollection<ImpCertificadoDeAnalisis> ImpCertificadoDeAnalisiss { get; set; }
        public virtual ICollection<ImpAsigRecorrCtrolCalid> ImpAsigRecorrCtrolCalidd { get; set; }
        public virtual ICollection<ImpTicketPesadaBodega> ImpTicketPesadaBodega { get; set; }
        public virtual bool AnalisisAutomatico { get; set; }
        public virtual bool EnviaACamara { get; set; }
    }
}
