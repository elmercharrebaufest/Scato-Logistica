using System;

namespace Molinos.Scato.Dominio.Dto
{
	public class TicketPesadaDto
	{
		public string CTG { get; set; }
		public int? BrutoOrigen { get; set; }
		public int? TaraOrigen { get; set; }
		public int? NetoOrigen { get; set; }
        public int? BrutoPlanta{ get; set; }
        public int? TaraPlanta { get; set; }
        public int? NetoPlanta { get; set; }
        public DateTime? FechaHoraIngreso { get; set; }
		public DateTime? FechaHoraEgreso { get; set; }
		public string Procedencia { get; set; }
		public string IntermediarioFlete { get; set; }
		public string IntermediarioFleteCUIT { get; set; }
		public string Transportista { get; set; }
		public string TransportistaCUIT { get; set; }
        public string TitularCPCUIT { get; set; }
        public string Patente { get; set; }
		public string ChoferNombreApellido { get; set; }
		public string Material { get; set; }
	}
}
