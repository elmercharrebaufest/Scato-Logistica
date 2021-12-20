using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CambiarPinchazosComando : Comando
    {
        public PinchazosPorCaladaDto Dto { get; set; }
        public int CentroId { get; set; }
    }
}
