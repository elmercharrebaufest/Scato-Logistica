using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpTicketPesadaAduanaDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string NumeroDocumento { get; set; }
        public string Material { get; set; }
        public string PesoBruto { get; set; }
        public string PesoTara { get; set; }
        public string PesoNeto { get; set; }
        public string Transportista { get; set; }
        public string CuitTransportista { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string Observaciones { get; set; }
        public IdentidadDeCopia Identidad { get; set; }//original-duplicado-etc
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public virtual string Codigo { get; set; }

        public string CodigoAduana { get; set; }

        public string CertificadoDeHabilitacion { get; set; }

        public string CuitExportador { get; set; }

        public string RazonSocialExportador { get; set; }

        public string PermisoDeEmbarque { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaEgreso { get; set; }

        public string IdentificadorDeContenedor { get; set; }

        public string ChoferTipoDoc { get; set; }

        public string ChoferNumero { get; set; }

        public string ChoferApellido { get; set; }

        public string ChoferNombre { get; set; }

        public DateTime? VencimientoDeCertificacion { get; set; }

        public string NumeroInternoSap { get; set; }

        public string Nacionalidad { get; set; }

        public string NumeroIngreso { get; set; }

        public string BalanzaNombre { get; set; }

        public string Longitud { get; set; }

        public string Latitud { get; set; }

        public string Lot { get; set; }

        public DateTime? FechaHoraPesoTara { get; set; }

        public DateTime? FechaHoraPesoBruto { get; set; }
    }
}
