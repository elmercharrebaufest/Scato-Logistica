using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto.QRCamiones
{
	public class CamionQRCamiones
	{
		public string Patente { get; set; }
		public string PatenteAcoplado { get; set; }
	}

	public class ChoferQRCamiones
	{
		public string CUIL { get; set; }
		public string TipoDocumento { get; set; }
		public string NumeroDocumento { get; set; }
		public bool Extranjero { get; set; }
		public string NombreApellido { get; set; }
	}

	public class DatosAdicionalesQRCamiones
	{
		public string PreCaladoFila { get; set; }
		public string PostCaladoFila { get; set; }
		public string CaladoEstado { get; set; }
		public int? PesadaBruto { get; set; }
		public int? PesadaTara { get; set; }
		public int? PesadaDescargado { get; set; }
	}

	public class EtapaQRCamiones
	{
		public string Nombre { get; set; }
		public DateTime Fecha { get; set; }
		public EstadoEtapaQRCamiones Estado { get; set; }
		public string NombreTabla { get; set; }
	}

	public enum EstadoEtapaQRCamiones
	{
		Completado,
		EnProceso,
		Pendiente
	}

	public class TrackingDataQRCamiones
	{
		public string Workflow { get; set; }
		public string CTG { get; set; }
		public DateTime FechaHoraIngreso { get; set; }
		public string TitularCartaPorte { get; set; }
		public string RemitenteComercialProd { get; set; }
		public string RemitenteComercialVtaPrim { get; set; }
		public string Entregador { get; set; }
		public string Transportista { get; set; }
		public string Material { get; set; }
		public bool Rechazado { get; set; }
		public CamionQRCamiones Camion { get; set; }
		public ChoferQRCamiones Chofer { get; set; }
		public DatosAdicionalesQRCamiones DatosAdicionales { get; set; }
		public List<EtapaQRCamiones> Etapas { get; set; }
	}
}