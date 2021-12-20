using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DistribucionDeAlmacenesDto
    {
        public int Id { get; private set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        public string Centro { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "RemitoBodegaTerceros")]
        public string NumeroDeDocumentoDeIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        public string Patente { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "LitrosRestantes")]
        public int PesoNetoBodegaEnLitros { get; set; }

        public Guid InstanceId { get; set; }
        public string NombreDeUsuario { get; set; }
        public DateTime Fecha { get; set; }

        public string Workflow { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public int MaterialId { get; set; }

        public string DistribucionesDeAlmacenesJson { get; set; }
        public List<DistribucionDeAlmacenDto> DistribucionesDeAlmacenes { get; set; }
    }
}