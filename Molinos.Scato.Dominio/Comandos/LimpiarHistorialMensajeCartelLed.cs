namespace Molinos.Scato.Dominio.Comandos
{
    public class LimpiarHistorialMensajeCartelLed : Comando
    {
        public string Codigo { get; set; }

        public int CalleId { get; set; }
        public bool LimpiarCamion { get; set; }
        public int HistorialMensajeCartelLedId { get; set; }
        public bool UltimoCamion { get; set; }
    }
}