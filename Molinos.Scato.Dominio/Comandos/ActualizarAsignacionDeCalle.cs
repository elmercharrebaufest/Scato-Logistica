using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarAsignacionDeCalle : Comando
    {
        public int CalleId { get; set; }
        public int[] HidraulicasId { get; set; }
    }
}
