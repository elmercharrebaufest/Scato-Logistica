using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AdicionalesCartaPorteByPassDto
    {
        public int? EstablecimientoId { get; set; }
        public int? AlmacenId { get; set; }
        public string NombreUsuario { get; set; }
        public CaladoDto Calado { get; set; }
        public int RecorridoIdIngreso { get; set; }
    }
}