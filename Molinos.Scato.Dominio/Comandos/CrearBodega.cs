using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearBodega : Comando
    {
        public BodegaDto Dto { get; set; }
    }
}
