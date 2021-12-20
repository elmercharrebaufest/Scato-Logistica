using System;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpTicketPesadaAduana")]
    public class ImpTicketPesadaAduana : Impresion
    {
        public virtual string NumeroDocumento { get; set; }
        public virtual string Material { get; set; }
        public virtual string PesoBruto { get; set; }
        public virtual string PesoTara { get; set; }
        public virtual string PesoNeto { get; set; }
        public virtual string Transportista { get; set; }
        public virtual string CuitTransportista { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual IdentidadDeCopia Identidad { get; set; }//original-duplicado-etc

        public virtual string CodigoAduana { get; set; }


        public virtual string CertificadoDeHabilitacion { get; set; }
        public virtual string CuitExportador { get; set; }
        public virtual string RazonSocialExportador { get; set; }
        public virtual string PermisoDeEmbarque { get; set; }
        public virtual DateTime? FechaInicio { get; set; }
        public virtual DateTime? FechaEgreso { get; set; }
        public virtual string IdentificadorDeContenedor { get; set; }
        public virtual string ChoferTipoDoc { get; set; }
        public virtual string ChoferNumero { get; set; }
        public virtual string ChoferApellido { get; set; }
        public virtual string ChoferNombre { get; set; }
        public virtual DateTime? VencimientoDeCertificacion { get; set; }
        public virtual string NumeroInternoSap { get; set; }
        public virtual string Nacionalidad { get; set; }
        public virtual string NumeroIngreso { get; set; }
        public virtual string BalanzaNombre { get; set; }
        public virtual string Longitud { get; set; }
        public virtual string Latitud { get; set; }
        public virtual string Lot { get; set; }
        public virtual DateTime? FechaHoraPesoTara { get; set; }
        public virtual DateTime? FechaHoraPesoBruto { get; set; }
       
    }
}
