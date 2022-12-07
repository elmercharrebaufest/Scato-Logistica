using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCargaDeCupo : Comando
    {
        public CargaDeCupoDto Dto { get; set; }
    }
}
