using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.ViewModel
{
    public class AutomatismoNoGranoViewModel
    {
        public AutomatismoNoGranoViewModel()
        {
            AutomatismoNoGrano = new AutomatismoNoGranoDto();
        }

        public AutomatismoNoGranoDto AutomatismoNoGrano { get; set; }
        public ListaPaginada<AutomatismoNoGranoDto> ListaAutomatismoNoGrano { get; set; }
        public List<SelectListItem> CallesPlanta { get; set; }
        public List<SelectListItem> CallesPlayaInterna { get; set; }
        public List<SelectListItem> PuntosDeCarga { get; set; }
        public List<SelectListItem> Almacenes { get; set; }
    }
}