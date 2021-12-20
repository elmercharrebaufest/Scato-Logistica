using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaMangueraCargaDto
    {
        public int Id { get; set; }
        public DateTime? FechaConexionMangueras { get; set; }
        public string HoraConexionMangueras { get; set; }
        public DateTime? FechaDesconexionMangueras { get; set; }
        public string HoraDesconexionMangueras { get; set; }
        public DateTime? FechaComienzoCarga { get; set; }
        public string HoraComienzoCarga { get; set; }
        public DateTime? FechaFinalizacionCarga { get; set; }
        public string HoraFinalizacionCarga { get; set; }
    }
}