namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEstadoCallePreHidraulica : Comando
    {
        public int Id { get; set; }
        public bool ActivoAutomatico { get; set; }
    }
}