
using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class SincronizarResultadosCamara : Comando
    {
        public List<ResultadoCamaraDto> Resultados { get; set; }
    }
}
