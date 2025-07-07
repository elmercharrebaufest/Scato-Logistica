using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class DatosRecorridoDto
    {
        public Guid InstanciaWorkflow { get; set; }
        public string TarjetaDeAcceso { get; set; }
        public string Patente { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public string CentroCodigoSap { get; set; }
        public int WorkflowId { get; set; }

        public int Id { get; set; }
        public bool AdvertirCaladoEnPlanta { get; set; }
        public string CartaDePorte { get; set; }
        public bool Entregador { get; set; }
        public string Material { get; set; }
        public int? PesoTaraOrigen { get; set; }
        public int? PesoBrutoOrigen { get; set; }
        public int? PesoBruto { get; set; }
        public int? PesoTara { get; set; }
        public int? PesoNetoOrigen { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
        public TipoDeWorkflow TipoDeWorkflow { get; set; }
        public bool SinRecorrido { get; set; }

        public string ProximaAccion { get; set; }
        public string ProximaAccionMensaje { get; set; }
        public string Calle { get; set; }
        public TipoDocumentoIngreso TipoDocumento { get; set; }
        public string TipoComercial { get; set; }
        public string PatentePrevia { get; set; }
        public bool PasoPorContingenciaPesosExc { get; set; }
        public int MaterialId { get; set; }
        public int CentroId { get; set; }
        public string PatenteAcoplado { get; set; }
        public string Ctg { get; set; }
    }
}
