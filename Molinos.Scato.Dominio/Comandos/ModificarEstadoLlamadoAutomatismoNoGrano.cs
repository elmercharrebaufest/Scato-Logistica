namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEstadoLlamadoAutomatismoNoGrano : Comando
    {
        public int Id { get; set; }
        public bool Estado { get; set; }
    }
}