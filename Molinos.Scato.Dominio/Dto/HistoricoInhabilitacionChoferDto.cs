using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class HistoricoInhabilitacionChoferDto
    {

        public int Id { get; set; }

        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }

        public int InhabilitacionChoferId { get; set; }

        public string NombreUsuarioResponsable { get; set; }
        public string NombreUsuarioCambio { get; set; }

        public string Motivo { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }

        public List<HistoricoInhabilitacionChoferDto> Historico { get; set; }
        public List<AutorizacionChoferDto> Autorizacion { get; set; }

    }
}
