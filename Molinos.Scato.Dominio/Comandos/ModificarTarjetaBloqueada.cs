using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarTarjetaBloqueada : Comando
    {
        public TarjetaBloqueadaDto Dto { get; set; }
    }
}
