using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearAutorizarTiempoEnTransito : Comando
    {
        public AutorizacionTiempoEnTransitoDto Dto { get; set; }
    }
}
