using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class EstacionMeteorologicaDto
    {
        public string Descripcion { get; set; }
        public string[] Detalle { get; set; }
        public byte[][] Imagenes { get; set; }
    }
}
