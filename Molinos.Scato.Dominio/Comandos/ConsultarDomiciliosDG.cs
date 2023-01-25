namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarDomiciliosDG : Comando
    {
        public int CentroId { get; set; }
        public long Cuit { get; set; }
    }
}