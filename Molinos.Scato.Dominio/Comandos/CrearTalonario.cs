using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{

    public class CrearTalonario: Comando
    {
        public TalonarioDto Dto { get; set; }
    }
}
