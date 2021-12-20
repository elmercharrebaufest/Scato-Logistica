using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CargaFiltroDto
    {
        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_NumeroPesada")]
        public int? Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_NumeroBalanza")]
        public string NumeroBalanza { get; set; }

        public int? IdVapor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Buque")]
        public string VaporDesc { get; set; }

        public int? IdMaterial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Commodity")]
        public string MaterialDesc { get; set; }

        public int? IdBodega { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Bodega")]
        public string BodegaDesc { get; set; }

        public int? IdExportador { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Exportador")]
        public string ExportadorDesc { get; set; }

        public int? IdDestino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Destino")]
        public string DestinoDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_NumeroPesada")]
        public int? IdFin { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_EnviadoASap")]
        public bool? EnviadoASap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_BalanzadasFaltantes")]
        public int? BalanzadaFaltante { get; set; }

        public ListaPaginada<BalanzadaDto> Balanzada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_FiltroCarga_Desde")]
        public DateTime? FechaDesde { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_FiltroCarga_Hasta")]
        public DateTime? FechaHasta { get; set; }

    }
}
