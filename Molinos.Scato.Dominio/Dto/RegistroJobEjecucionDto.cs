using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class RegistroJobEjecucionDto
    {
        public int Id { get; set; }
        public string NombreProceso { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEjecucion { get; set; }
    }
}