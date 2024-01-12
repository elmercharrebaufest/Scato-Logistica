namespace Molinos.Scato.Dominio.Comandos
{
    public class LimpiarRecorridoHistorialMensajeCartelLed : Comando
    {
        public int RecorridoId { get; set; }

        public string Codigo { get; set; }
    }
}