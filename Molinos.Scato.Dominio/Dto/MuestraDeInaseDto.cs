using Molinos.Scato.Dominio.Enums;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MuestraDeInaseDto
    {
        [DataMember]
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int RecorridoId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public int CentroId { get; set; }
        public DateTime FechaMuestra { get; set; }
        public string CartaPorte { get; set; }
        public string Patente { get; set; }
        public string MaterialDescripcion { get; set; }
        public string DestinatarioCuil { get; set; }
        public string RtteComercialCuit { get; set; }
        public string CorredorCuil { get; set; }
        public int PesoNeto { get; set; }
        public string TitularCartaPorte { get; set; }
        public bool? CPE { get; set; }
        public int? Sucursal { get; set; }
        public string CTG { get; set; }
        public string TitularCartaPorteCuil { get; set; }
        public string CodEstab { get; set; }
        public string Direccion { get; set; }
        public string ProcedenciaCodigoSap { get; set; }
        public string LocalidadCodigoSap { get; set; }
        public string Localidad { get; set; }
        public string Vendedor { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
        public int CantidadDeVagones { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string Corredor { get; set; }
        public string IntermediarioCuit { get; set; }
        public string Intermediario { get; set; }
        public string RtteComercial { get; set; }
        public string Cosecha { get; set; }
        public int? ProcedenciaCodigoPostal { get; set; }
        public int? ProcedenciaSubcodigoPostal { get; set; }
        public DateTime FechaDescarga { get; set; }
        public int NumeroVehiculo { get; set; }
        public string CodigoDeCamara { get; set; }
        public string CamaraDesc { get; set; }
        public CamaraFormatoDeArchivo CamaraFormatoDeArchivo { get; set; }
        public string CodigoCamaraGrupo { get; set; }
        public string CodigoCamaraMaterial { get; set; }
        public string NroMuestra
        {
            get
            {
                return CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BahiaBlanca
                              ? CartaPorte.Replace("-", "").Replace("R", "").Substring(CartaPorte.Length - 10)
                              : (CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.Rosario ?
                              (CodigoDeCamara != null ? CodigoDeCamara.Substring(0, CodigoDeCamara.Length > 3 ? 3 : CodigoDeCamara.Length) : "") :
                              (CodigoDeCamara != null ? CodigoDeCamara.Substring(0, CodigoDeCamara.Length > 2 ? 2 : CodigoDeCamara.Length) : ""))
                              + NumeroVehiculo.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') + CartaPorte.Replace("-", "").Replace("R", "").Substring(CartaPorte.Length - 10);
            }
        }
    }
}
