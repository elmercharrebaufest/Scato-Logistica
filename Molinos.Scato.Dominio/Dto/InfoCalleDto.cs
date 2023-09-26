using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class InfoCalleDto
    {

        public string Descripcion { get; set; }

        public string TipoCalle { get; set; }

        public string Estado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "EsIncluidoAutomatizmo")]
        public string EsIncluidoAutomatizmo { get; set; }

        public string Material { get; set; }

        public string Variedad { get; set; }

        public string Hidraulica { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "EsPaseDirecto")]
        public string EsPaseDirecto { get; set; }
        public string Almacen { get; set; }
        public string PuntoDeCarga { get; set; }
    }
}
