using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class StockDeEstablecimientoDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento_Codigo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoEstablecimiento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Campaña")]
        [MaxLength(5, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Cosecha { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "FechaDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "StockDeclarado")]
        public decimal StockDeclarado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "StockUtilizado")]
        public decimal StockUtilizado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "StockDisponible")]
        public decimal StockDisponible { get { return StockDeclarado - StockUtilizado; } }
        [Display(ResourceType = typeof(Textos), Name = "StockReservado")]
        public decimal StockReservado { get; set; }
        [Display(ResourceType =typeof(Textos), Name = "NombreEstablecimeinto")]
        public string NombreEstablecimiento { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "StockTolerancia")]
        public decimal? Tolerancia { get; set; }

        public string Localidad { get; set; }
        public string LocalidadCodigoAfip { get; set; }
        public int? LocalidadId { get; set; }

        public string Provincia { get; set; }
        public int? ProvinciaCodigoAfip { get; set; }
        public int? ProvinciaId { get; set; }
        public bool EstablecimientoAnulado { get; set; }

        public StockDeEstablecimientoDto()
        {
            this.StockUtilizado = 0;
            this.StockDeclarado = 0;
            this.StockReservado = 0;
        }
    }
}
