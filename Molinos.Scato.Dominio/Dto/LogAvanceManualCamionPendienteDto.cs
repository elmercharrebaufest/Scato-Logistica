using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class LogAvanceManualCamionPendienteDto
    {
        public int    Id                { get; set; }
        public string PatenteLeida      { get; set; }
        public string PatenteIngresada   { get; set; }
        public string Tarjeta           { get; set; }
        public string NombrePuesto      { get; set; }
        public DateTime FechaEvento     { get; set; }
        public string NombreUsuario     { get; set; }
        public string MotivoFallo       { get; set; }
        /// <summary>0=Pendiente, 1=Editando, 2=Atendido</summary>
        public int    Atendido          { get; set; }
    }
}
