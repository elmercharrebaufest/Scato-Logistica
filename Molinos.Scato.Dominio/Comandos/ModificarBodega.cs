using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarBodega : Comando
    {
        public BodegaDto Dto { get; set; }
    }
}
