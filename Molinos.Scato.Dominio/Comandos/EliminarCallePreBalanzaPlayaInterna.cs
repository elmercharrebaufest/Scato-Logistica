namespace Molinos.Scato.Dominio.Comandos
{
    public class EliminarCallePreBalanzaPlayaInterna : Comando
    {
        public int CallePlayaInternaId { get; set; }
        public int CallePreBalanzaId { get; set; }
    }
}