using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearInhabilitacionCamion : Comando
    {
        public InhabilitacionCamionDto Dto { get; set; }
    }
}