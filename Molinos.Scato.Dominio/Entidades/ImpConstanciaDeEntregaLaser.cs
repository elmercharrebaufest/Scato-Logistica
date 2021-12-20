using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpConstanciaDeEntregaLaser")]
    public class ImpConstanciaDeEntregaLaser : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string CentroDireccion { get; set; }
        public virtual string CentroLocalidad { get; set; }
        public virtual string CentroProvincia { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string NumeroDeOrden { get; set; }
        public virtual string NumeroCertificacion { get; set; }
        public virtual string Material { get; set; }
        public virtual string Corredor { get; set; }
        public virtual string Vendedor { get; set; }
        public virtual string Procedencia { get; set; }
        public virtual string NumeroCartaPorte { get; set; }
        public virtual string PesoBruto { get; set; }
        public virtual string PesoTara { get; set; }
        public virtual string PesoNeto { get; set; }
        public virtual DateTime FechaCartaPorte { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual string ObservacionesCalado { get; set; }
        public virtual string BalanzaTara { get; set; }
        public virtual string BalanzaBruto { get; set; }
        public virtual string ModeloBalanzaTara { get; set; }
        public virtual string ModeloBalanzaBruto { get; set; }
        public virtual string NroSerieBalanzaTara { get; set; }
        public virtual string Entregador { get; set; }

        public virtual string NroSerieBalanzaBruto { get; set; }
        [InverseProperty("ImpConstanciaDeEntregaLaser")]
        public virtual ICollection<CaracteristicasCalidadValor> CaracteristicasCalidadValor { get; set; }
    }
}
