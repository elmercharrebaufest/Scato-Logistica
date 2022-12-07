using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarLlamadoAutomaticoHidraulica : Comando
    {
        public int Id { get; set; }
        public EstadoHidraulica Estado { get; set; }
        public string Patente { get; set; }
    }
}
