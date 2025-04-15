using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ValidarOrdenCargaInternaFas : Comando
    {
        public OrdenDeCargaDto Orden { get; set; }
        public int CentroId { get; set; }
        public bool AplicaFastPass { get; set; }
    }
}
