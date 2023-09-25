namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEstadoEscalableHidraulica : Comando
    {
        public int Id { get; set; }
        public bool EsEscalable { get; set; }
    }
}