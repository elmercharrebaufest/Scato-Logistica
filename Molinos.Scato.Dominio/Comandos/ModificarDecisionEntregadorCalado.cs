namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarDecisionEntregadorCalado : Comando
    {
        public int CaladoId { get; set; }
        public bool Decision { get; set; }
    }
}
