namespace Molinos.Scato.Dominio.Comandos
{
    public class SuscribirDispositivos : Comando
    {
        public string Codigo { get; set; }
        public string Evento { get; set; }
        public bool RutaWeb { get; set; }

    }
}
