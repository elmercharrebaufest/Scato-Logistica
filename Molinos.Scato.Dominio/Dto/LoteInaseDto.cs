using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LoteInaseDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Lote_NroLote")]
        public string NumeroDeLote { get; set; }
        public List<MuestraDeInaseDto> Muestras { get; set; }
        public DateTime Fecha { get; set; }
        public int CentroId { get; set; }
        public string NombreUsuario { get; set; }
    }
}
