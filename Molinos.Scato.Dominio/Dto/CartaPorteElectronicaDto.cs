using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CartaPorteElectronicaDto
    {
        public int Id { get; set; }
        public int? TipoCartaPorte { get; set; }
        public int MaterialId { get; set; }
        public int CentroId { get; set; }
        public string NroOrden { get; set; }
        public int Tipo { get; set; }
        public long NroCtg { get; set; }
        public int? Cosecha { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public DateTime? FechaCacheado { get; set; }
        public byte[] Pdf { get; set; }
        public long CuitOrigen { get; set; }
        public int PesoBruto { get; set; }
        public long? CuitTransportista { get; set; }
        public string Dominio { get; set; }
        public string NroRenspa { get; set; }
    }
}