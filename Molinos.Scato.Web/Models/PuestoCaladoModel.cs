namespace Molinos.Scato.Web.Models
{
    public class PuestoCaladoModel
    {
        public int Id { get; set; }
        public string NombrePuesto { get; set; }
        public bool EstadoConexion { get; set; }
        public string MensajeConexion { get; set; }
        public int ColaIdentificacionVehicularId { get; set; }
        public string Patente { get; set; }
        public bool ReconocimientoExitoso { get; set; }
        public string MensajeError { get; set; }
        public string ImagenBase64 { get; set; }
    }
}