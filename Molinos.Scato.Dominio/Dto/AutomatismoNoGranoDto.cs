using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Molinos.Scato.Dominio.Dto
{
    public class AutomatismoNoGranoDto : AuditoriaBaseDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePlanta")]
        public CalleDto CallePlanta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePlanta")]
        public int CallePlantaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePlayaInterna")]
        public CalleDto CallePlayaInterna { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePlayaInterna")]
        public int CallePlayaInternaId { get; set; }

        public bool Activo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuntodeCarga")]
        public PuntoDeCargaDto PuntoDeCarga { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuntodeCarga")]
        public int PuntoDeCargaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        public AlmacenDto Almacen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        public int AlmacenId { get; set; }
    }
}