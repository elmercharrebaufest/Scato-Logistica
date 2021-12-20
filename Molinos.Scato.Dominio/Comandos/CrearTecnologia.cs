using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearTecnologia : Comando
    {
        public TecnologiaDto Dto { get; set; }
    }
}
