namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarLecturaDeTarjetaPatente : Comando
    {
        public int PuestoDeTrabajoId { get; set; }
        public string Patente { get; set; }
        public string PatenteLeida { get; set; }
        public string Lectura { get; set; }
        public float? Certeza { get; set; }
        public string CodigoDispositivo { get; set; }
        public string FotoRuta { get; set; }
    }
}
