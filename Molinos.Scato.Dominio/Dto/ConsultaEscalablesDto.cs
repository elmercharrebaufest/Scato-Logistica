using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class ConsultaEscalablesDto
    {
        public string Result { get; set; }
        public string Status { get; set; }
        public ConsultaEscalableValoresDto Data { get; set; }

    }
}
