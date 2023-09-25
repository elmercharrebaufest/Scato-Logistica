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
        public List<PuntoDeCargaDto> PuntosDeCargaAsociados { get; set; }

        public string ListaPuntosDeCarga
        {
            get
            {
                return String.Join(",", this.PuntosDeCargaAsociados.ToList().Select(x => x.Descripcion));
            }
        }

        [Display(ResourceType = typeof(Textos), Name = "PuntodeCarga")]
        public List<int> PuntosDeCargaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        public List<AlmacenDto> AlmacenesAsociados { get; set; }

        public string ListaAlmacenes
        {
            get
            {
                return String.Join(",", this.AlmacenesAsociados.ToList().Select(x => x.Descripcion));
            }
        }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        public List<int> AlmacenesId { get; set; }
    }
}