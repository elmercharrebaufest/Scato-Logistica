using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Balanza : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Nombre { get; set; }
        public virtual string NombreCorto { get; set; }
        public virtual string Color { get; set; }
        [Required]
        public virtual TipoBalanza TipoBalanza { get; set; }
        [Required]
        public virtual int CentroEmisor { get; set; }
        public virtual Centro Centro { get; set; }

        public virtual int? ToleranciaOrigen { get; set; }
        public virtual int? ToleranciaRechazo { get; set; }
        public virtual decimal? ToleranciaxMil { get; set; }
        [Required]
        public virtual int ToleranciaIndianapolis { get; set; }

        [Required]
        public virtual string CodigoCabezal { get; set; }

        public virtual TipoAcceso TipoAcceso { get; set; }
        [Required]
        public virtual int MaximoValorCereo { get; set; }
        public virtual Modalidad Modalidad { get; set; }
        public virtual bool EstaEnCero { get; set; }
        public virtual string PuestoDeTrabajo { get; set; }

        public virtual string Modelo { get; set; }
        public virtual string NroSerie { get; set; }

        public virtual string CodigoLongitud { get; set; }
        public virtual string CodigoLatitud { get; set; }
        public virtual string CodigoLot { get; set; }
        public virtual string CertificadoDeHabilitacion { get; set; }
        public virtual DateTime? VencimientoDeCertificado { get; set; }

        public virtual TipoVehiculo TipoVehiculo { get; set; }
        public virtual bool EsExportacion { get; set; }

        public virtual bool Desactivado { get; set; }
        public virtual bool ContingenciaExcedentesHabilitada { get; set; }
        public virtual int? ToleranciaExcedida { get; set; }
    }
}

