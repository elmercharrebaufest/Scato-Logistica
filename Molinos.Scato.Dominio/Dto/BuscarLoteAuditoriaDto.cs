using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BuscarLoteAuditoriaDto
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int CamaraId { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public string NroLote { get; set; }
        public int LoteId { get; set; }
        public int CentroId { get; set; }
        public string ArchivoRutaDestino { get; set; }

    }
}