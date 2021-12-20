using System;
using System.Collections.Generic;
using System.Globalization;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MuestraEnvioACamaraBiotecnoligiaDto
    {
        private string nroCartaPorte;
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public int CamaraId { get; set; }
        public string CamaraDesc { get; set; }
        public string CodigoDeCamara { get; set; }
        public CamaraFormatoDeArchivo CamaraFormatoDeArchivo { get; set; }
        public string NroMuestra
        {
            get
            {
                return CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BahiaBlanca
                              ? NroCartaPorte.Replace("-", "").Replace("R", "").Substring(NroCartaPorte.Length - 10)
                              : (CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.Rosario ?
                              (CodigoDeCamara != null ? CodigoDeCamara.Substring(0, CodigoDeCamara.Length > 3 ? 3 : CodigoDeCamara.Length) : "") :
                              (CodigoDeCamara != null ? CodigoDeCamara.Substring(0, CodigoDeCamara.Length > 2 ? 2 : CodigoDeCamara.Length) : ""))
                              + NumeroVehiculo.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') + NroCartaPorte.Replace("-", "").Replace("R", "").Substring(NroCartaPorte.Length - 10);
            }
        }
        public string NroMuestraTerceros { get; set; }
        public string NroCartaPorte
        {
            get { return (nroCartaPorte ?? "0").Replace("-", "").Replace("R", "").PadLeft(12, '0'); }
            set { nroCartaPorte = value; }
        }

        public int NumeroVehiculo { get; set; }
        public DateTime FechaDescarga { get; set; }
        public int? PesoNeto { get; set; }
        public int CorredorId { get; set; }
        public int CentroId { get; set; }
        public string Corredor { get; set; }
        public int VendedorId { get; set; }
        public string Vendedor { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public string Localidad { get; set; }
        public string Patente { get; set; }
        public IList<CaracteristicaDeCalidadDto> CaracteristicasDeCalidad { get { return new List<CaracteristicaDeCalidadDto>(); } }
        public Guid WorkflowInstanceId { get; set; }
        public string Actividad { get; set; }
        public int? LoteId { get; set; }
        public EstadoMuestra EstadoMuestra { get; set; }
        public DateTime? FechaCartaPorte { get; set; }
        public string Proveedor { get; set; }
        public string ProveedorCodigoSap { get; set; }
        public bool TieneAnalisisInterno { get; set; }
        public TipoDocumentoIngreso TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public bool GeneroMicroMuestras { get; set; }

        public string RtteComercialCuit { get; set; }

        public string TitularCartaPorteCuil { get; set; }

        public string CorredorCuil { get; set; }

        public string RtteComercial { get; set; }

        public string TitularCartaPorte { get; set; }

        public string CodigoCamaraMaterial { get; set; }

        public string CodigoCamaraGrupo { get; set; }

        public string CTG { get; set; }

        public bool CPE { get; set; }

        public string CodEstab { get; set; }

        public string Direccion { get; set; }

        public string ProcedenciaCodigoSap { get; set; }

        public string LocalidadCodigoSap { get; set; }

        public string DestinatarioCodigoSap { get; set; }

        public string Destinatario { get; set; }

        public string DestinatarioCuil { get; set; }

        public int CantidadDeVagones { get; set; }

        public string CodigoEstablecimiento { get; set; }

        public TipoVehiculo TipoVehiculo { get; set; }

        public int TitularCartaPorteId { get; set; }

        public int DestinatarioId { get; set; }

        public string TitularCartaPorteMail { get; set; }

        public string DestinatarioMail { get; set; }

        public int RtteComercialId { get; set; }

        public string RtteComercialMail { get; set; }

        public int? Caratula { get; set; }

        public string CentroCodigoPostal { get; set; }

        public DateTime FechaEmision { get; set; }

        public string Cosecha { get; set; }

        public string Entregador { get; set; }

        public string Procedencia { get; set; }

        public string NroSecArchivoSolicitudes { get; set; }

        public string CodigoTecnologia { get; set; }

        public string Intermediario { get; set; }

        public string IntermediarioCuit { get; set; }
        public int? Sucursal { get; set; }
    }
}
