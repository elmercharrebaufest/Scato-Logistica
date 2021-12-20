using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpCIUDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string NroComprobante { get; set; }
        public string Fecha { get; set; }
        public string RazonSocialBodega { get; set; }
        public string INVBodega { get; set; }
        public string CUITBodega { get; set; }
        public string IIBBBodega { get; set; }
        public string RazonSocialVinedo { get; set; }
        public string INVVinedo { get; set; }
        public string CUITVinedo { get; set; }
        public string IIBBVinedo { get; set; }
        public string PesoBrutoBodega { get; set; }
        public string PesoTaraBodega { get; set; }
        public string PesoNetoBodega { get; set; }
        public string TipoVehiculo { get; set; }
        public string Patente { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Chofer { get; set; }
        public string Variedad { get; set; }
        public string TenorAzucarino { get; set; }
        public string ModalidadComercializacion { get; set; }
        public string Observaciones { get; set; }

        public IdentidadDeCopia Identidad { get; set; }//original-duplicado-etc
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public virtual string Codigo { get; set; }
    }
}
