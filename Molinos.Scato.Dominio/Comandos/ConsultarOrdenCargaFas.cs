using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarOrdenCargaFas : Comando
    {
        public OrdenDeCargaSapDto Orden { get; set; }
    }
}