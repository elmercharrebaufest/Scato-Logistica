namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarBalanzaEstaEnCero : Comando
    {
        public int BalanzaId { get; set; }
        public bool EstaEnCero { get; set; }
    }
}
