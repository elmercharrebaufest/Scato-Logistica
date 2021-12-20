using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DensidadPorTemperaturaDeMaterialDto
    {
        public int Id { get; set; }
        public int Grado { get; set; }
        public MaterialDto Material { get; set; }
        public decimal Densidad { get; set; }
    }
}
