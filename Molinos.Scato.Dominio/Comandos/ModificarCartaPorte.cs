using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCartaPorte : Comando
    {
        public CartaPorteDto Orden { get; set; }
        public string NombreUsuario { get; set; }
    }
}
