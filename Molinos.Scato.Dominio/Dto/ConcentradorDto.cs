using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConcentradorDto
    {
        public int PuestoId { get; set; }
        public string Concentrador { get; set; }
        public List<DispositivoGenericoDto> Sensores { get; set; }
        public EstadoSensoresBalanzaDto EstadoSensoresBalanzaDto{get;set;}
    }
}
