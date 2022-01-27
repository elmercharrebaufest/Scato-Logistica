using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class RegistroInactividadDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public int? MotivoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public int? PuestoTrabajoId { get; set; }
    }
}
