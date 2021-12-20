using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarTaraContenedor : Comando
    {
        public TaraContenedorDto Dto { get; set; }
    }
}
