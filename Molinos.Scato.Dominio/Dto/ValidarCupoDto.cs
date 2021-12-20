using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ValidarCupoDto
    {
        public bool Valido { get; set; }

        public CargaDeCupoDto Reingresado { get; set; }
        public bool YaAsignado { get; set; }
        public string MensajeError { get; set; }
    }
}