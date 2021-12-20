using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public partial class CrearInhabilitacionChofer : Comando
    {
        public InhabilitacionChoferDto Dto { get; set; }
    }
}