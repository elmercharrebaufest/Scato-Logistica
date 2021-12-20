using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ContingenciaDto
    {
        public string TipoContingencia { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public bool Activado { get; set; }
        public string Motivo { get; set; }
    }
}