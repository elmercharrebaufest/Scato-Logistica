namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEstadoAutomatismoNoGrano : Comando
    {
        public int Id { get; set; }
        public bool Estado { get; set; }
    }
}