using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class OtroRecorridoDelChoferDto
    {
        public string Patente { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        
    }
}
