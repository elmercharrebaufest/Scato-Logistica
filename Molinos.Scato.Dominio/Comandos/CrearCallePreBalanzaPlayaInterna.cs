namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCallePreBalanzaPlayaInterna : Comando
    {
        public int CallePlayaInternaId { get; set; }
        public int CallePreBalanzaId { get; set; }
        public string CodigoAutomatismoTipoLlamado { get; set; }
        public int? RecorridoId { get; set; }
        public bool EsCamionEnEspera { get; set; }
    }
}