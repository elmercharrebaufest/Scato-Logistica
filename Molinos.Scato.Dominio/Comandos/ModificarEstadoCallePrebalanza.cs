namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEstadoCallePrebalanza : Comando
    {
        public int Id { get; set; }
        public bool ActivoAutomatico { get; set; }
    }
}