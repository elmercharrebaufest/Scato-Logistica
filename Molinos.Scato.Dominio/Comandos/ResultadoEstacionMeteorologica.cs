using Molinos.Scato.Dominio.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ResultadoEstacionMeteorologica : Resultado
    {
        public List<EstacionMeteorologicaDto> Imagenes { get; set; }
    }
}
