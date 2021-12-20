using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LoteBiotecnologiaDto
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string Material { get; set; }
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        public string CamaraDesc { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CamaraId { get; set; }

        public string CamaraEmail { get; set; }

        public CamaraFormatoDeArchivo CamaraFormatoDeArchivo { get; set; }

        public IList<MuestraEnvioACamaraBiotecnoligiaDto> Muestras { get; set; }

        public string NombreUsuario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Lote_NroLote")]
        public string NumeroDeLote { get; set; }

        public int CentroId { get; set; }

        public string CentroDesc { get; set; }
    }
}
