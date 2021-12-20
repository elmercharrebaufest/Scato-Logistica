using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MuestraEnvioACamaraDto
    {
        private string nroCartaPorte;
        //Mapeo Directo
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public int CaladoId { get; set; }
        public int CamaraId { get; set; }
        public string NroMuestra { get; set; }
        public string NroMuestraTerceros { get; set; }
        public string NroCartaPorte
        {
            get { return (nroCartaPorte ?? "0").Replace("-", "").Replace("R", "").PadLeft(12,'0'); }
            set { nroCartaPorte = value; }
        }

        public DateTime? FechaDescarga { get; set; }
        public int? PesoNeto { get; set; }
        public int CentroId { get; set; }
        public string Localidad { get; set; }
        public string Patente { get; set; }
        public IList<CaracteristicaDeCalidadDto> CaracteristicasDeCalidad { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public EstadoMuestra EstadoMuestra { get; set; }
        public bool TieneAnalisisInterno { get; set; }
        public TipoDocumentoIngreso TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public bool GeneroMicroMuestras { get; set; }
        public bool HuboExcepcion { get; set; }
        
        //Listar muestras consulta
        public string Actividad { get; set; }
        public string Material { get; set; }
        public string Vendedor { get; set; }
        public string Corredor { get; set; }
        public string Proveedor { get; set; }
        public string ProveedorCodigoSap { get; set; }
        public DateTime FechaCartaPorte { get; set; }
        public string CamaraDesc { get; set; }

        //Listar muestra para archivo
        public string MaterialCodigoCamara { get; set; }
        public int? TitularCartaPorteId { get; set; }
        public string TitularCartaPorteCuil { get; set; }
        public string TitularCartaPorteMail { get; set; }
        public string TitularCartaPorte { get; set; }
        public int? DestinatarioId { get; set; }
        public string Destinatario { get; set; }
        public string DestinatarioCuil { get; set; }
        public string DestinatarioMail { get; set; }
        public int? CorredorId { get; set; }
        public string CorredorCuil { get; set; }
        public int? RtteComercialId { get; set; }
        public string RtteComercial { get; set; }
        public string RtteComercialCuit { get; set; }
        public string RtteComercialMail { get; set; }
        public int? Caratula { get; set; }
        public string CentroCodigoPostal { get; set; }
        public string CentroCodigoCamara { get; set; }
        public string CodigoTecnologia { get; set; }
        public string GrupoCodigoCamara { get; set; }
        public string CTG { get; set; }
        public bool CPE { get; set; }
        public string CodEstab { get; set; }
        public string Direccion { get; set; }
        public string ProcedenciaCodigoSap { get; set; }
        public string LocalidadCodigoSap { get; set; }
        public DateTime PesoNetoFecha { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
        public int CantidadVehiculos { get; set; }
        public string CentroDestinoCodigoEstablecimiento { get; set; }
        public string DestinatarioCodigoSap { get; set; }
        public string Procedencia { get; set; }
        public string Entregador { get; set; }
        public string Intermediario { get; set; }
        public string IntermediarioCuit { get; set; }
        public string Cosecha { get; set; }

        public IEnumerable<CaracteristicaDeCalidadDto> Caracteristicas { get; set; }

        public int NumeroVehiculo { get; set; }
        public int? Sucursal { get; set; }
    }
}
