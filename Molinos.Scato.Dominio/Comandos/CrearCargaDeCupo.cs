using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCargaDeCupo : Comando
    {
        public CargaDeCupoDto Dto { get; set; }

        public bool EsGarita { get; set; }
    }
}
