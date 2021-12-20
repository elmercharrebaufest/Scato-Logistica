using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Entidades;
using System;

namespace Molinos.Scato.Web.Models
{
    public class FiltroReportePesadaSeisHoras
    {
        public bool TodosExportador { get; set; }

        public string Exportador { get; set; }

        public int? Exportador_Id { get; set; }

        public bool TodosMaterial { get; set; }

        public string Material { get; set; }

        public int? Material_Id { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }
    }
}