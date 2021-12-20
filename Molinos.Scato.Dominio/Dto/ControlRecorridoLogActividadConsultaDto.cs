using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ControlRecorridoLogActividadConsultaDto
    {
        public string Tabla { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }
        public string Actividad { get; set; }
        public string Usuario { get; set; }
    }
}
