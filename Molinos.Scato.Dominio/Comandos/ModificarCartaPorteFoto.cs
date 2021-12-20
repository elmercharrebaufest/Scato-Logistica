using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCartaPorteFoto : Comando
    {
        public CartaPorteDto Orden { get; set; }
    }
}
