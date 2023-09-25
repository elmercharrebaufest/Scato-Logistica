namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarPasoDirectoAutomatismoGrano : Comando
    {
        public int Id { get; set; }
        public bool EsPasoDirecto { get; set; }
    }
}