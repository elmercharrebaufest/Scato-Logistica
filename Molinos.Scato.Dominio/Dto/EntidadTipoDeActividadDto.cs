using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class EntidadTipoDeActividadDto
    {
        public int Id { get; set; }
        public int EntidadId { get; set; }
        public int TipoDeActividadId { get; set; }
        public EntidadDto Entidad { get; set; }
        public TipoDeActividadDto TipoDeActividad { get; set; }
    }
}
