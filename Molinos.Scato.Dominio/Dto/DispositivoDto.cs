using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DispositivoGenericoDto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
}
