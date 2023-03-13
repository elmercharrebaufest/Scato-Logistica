using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MuestraEnvioACamara : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string NombreUsuario { get; set; }

        public virtual CartaPorte CartaPorte { get; set; }

        public string NroMuestraTerceros { get; set; }

        public virtual DateTime? FechaDescarga { get; set; }

        public virtual int? PesoNeto { get; set; }

        public virtual string NroMuestra { get; set; }

        public virtual Calado Calado { get; set; }

        public virtual IList<CaracteristicaDeCalidad> CaracteristicasDeCalidad { get; set; }

        public virtual Camara Camara { get; set; }

        public virtual Lote Lote { get; set; }

        public virtual EstadoMuestra EstadoMuestra { get; set; }

        public virtual string Patente { get; set; }

        public virtual bool TieneAnalisisInterno { get; set; }

        public virtual TipoDocumentoIngreso? TipoDocumento { get; set; }

        public virtual string NroDocumento { get; set; }

        public virtual Centro Centro { get; set; }

        public virtual bool GeneroMicroMuestras { get; set; }

        public virtual bool HuboExcepcion { get; set; }
        public bool? EsPreLote { get; set; }

    }
}
