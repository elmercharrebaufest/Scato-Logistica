namespace Molinos.Scato.Dominio.Dto
{
    public class ColaIdentificacionVehicularDto
    {
        public int Id { get; set; }
        public int PuestoDeTrabajoId { get; set; }
        public string Patente { get; set; }
        public bool ReconocimientoExitoso { get; set; }
        public string MensajeError { get; set; }
        public string ImagenBase64 { get; set; }
    }
}
