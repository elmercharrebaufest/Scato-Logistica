using Molinos.Scato.Dominio.Dto;


namespace Molinos.Scato.Dominio.Comandos
{
    public class LimpiarHistorialMensajeCartelLed : Comando
    {
        public string Codigo { get; set; }

        public int CalleId { get; set; }

        public int? OrdenCircular { get; set; }
    }
}
