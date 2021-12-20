using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarSuplencia : Comando
    {
        public SuplenciaDto Dto { get; set; }
    }
}
