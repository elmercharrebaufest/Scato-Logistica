namespace Molinos.Scato.Dominio.Dto
{
    public class EstadoConexionDto
    {
        public string Dispositivo { get; set; }
        public int PuestoDeTrabajoId { get; set; }
        public int CentroId { get; set; }
        public bool Estado { get; set; }
        public string Mensaje { get; set; }
    }
}
