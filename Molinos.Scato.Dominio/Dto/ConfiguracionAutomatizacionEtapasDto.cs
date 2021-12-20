using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConfiguracionAutomatizacionEtapasDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int CentroId { get; set; }
        public string Actividad { get; set; }
        public int MinutosEjecucion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Deshabilitada { get; set; }
    }
}
