using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class LogAvanceManualCamionItemDto
    {
        public int       Id                { get; set; }
        public string    PatenteLeida      { get; set; }
        public string    PatenteIngresada  { get; set; }
        public string    WorkflowNombre    { get; set; }
        public string    Transportista     { get; set; }
        public DateTime  FechaEvento       { get; set; }
        public int?      PuestoDeTrabajoId  { get; set; }
        public string    NombrePuesto      { get; set; }
        public string    NombreUsuario     { get; set; }
    }
}
