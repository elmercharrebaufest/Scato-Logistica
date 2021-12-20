using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AdjuntoDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Archivo")]
        public string Archivo { get; set; }

        public string Descripcion { get; set; }   

        public int InhabilitacionChoferId { get; set; }

        public int InhabilitacionCamionId { get; set; }

    }
}