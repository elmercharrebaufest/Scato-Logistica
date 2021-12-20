using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AlmacenarObservacion : Comando
    {
        public ObservacionDto Dto { get; set; }
    }
}
