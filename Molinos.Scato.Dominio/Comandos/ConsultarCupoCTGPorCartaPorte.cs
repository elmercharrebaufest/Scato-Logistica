
namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarCupoCTGPorCartaPorte : Comando
    {
        public string NumeroCartaPorte { get; set; }
        public int CentroId { get; set; }
    }
}