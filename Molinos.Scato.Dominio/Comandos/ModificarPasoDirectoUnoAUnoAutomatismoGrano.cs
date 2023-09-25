namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarPasoDirectoUnoAUnoAutomatismoGrano : Comando
    {
        public int Id { get; set; }
        public bool EsPasoDirecto { get; set; }

        public bool LlamadoUnoAUno { get; set; }
    }
}