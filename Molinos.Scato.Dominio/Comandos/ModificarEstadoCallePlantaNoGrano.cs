namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEstadoCallePlantaNoGrano : Comando
    {
        public int Id { get; set; }
        public bool ActivoAutomatico { get; set; }
    }
}