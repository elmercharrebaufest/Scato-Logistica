namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEstadoHidraulica : Comando
    {
        public int Id { get; set; }
        public int IdAutomatismo { get; set; }
        public bool ActivoAutomatico { get; set; }
    }
}