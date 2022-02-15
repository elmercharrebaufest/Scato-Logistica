using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CartaPorteElectronicaDto
    {
        public int Id { get; set; }
        public int? TipoCartaPorte { get; set; }
        public int MaterialId { get; set; }
        public int CentroId { get; set; }
        public string NroOrden { get; set; }
        public int Tipo { get; set; }
        public long NroCtg { get; set; }
        public int? Cosecha { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public DateTime? FechaCacheado { get; set; }
        public byte[] Pdf { get; set; }
    }
}