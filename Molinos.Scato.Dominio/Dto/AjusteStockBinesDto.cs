using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AjusteStockBinesDto: IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Observaciones")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(100, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Observaciones { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime Fecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Movimiento")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoDeWorkflow? Movimiento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoDeBin")]
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Chofer_Nombre")]
        public string CentroDescripcion { get; set; }
        public int? CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_Nombre")]
        public string ProveedorDescripcion { get; set; }
        public int? ProveedorId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_Nombre")]
        public string VinedoPropioDescripcion { get; set; }
        public int? VinedoPropioId { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Tipo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoStockBines TipoAjuste { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Nuevo_Stock")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int Stock { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            if (Stock <= 0)
            {
                yield return new ValidationResult(Textos.IngresoBinSalida_CantidadNoNumerico, new[]{"Stock"});
            }

            if ((!string.IsNullOrEmpty(CentroDescripcion) && CentroId == 0 && TipoAjuste == TipoStockBines.Centro) )
            {
                yield return new ValidationResult(string.Format(Textos.Campo_Invalido, Textos.CentroINV), new[] { "CentroDescripcion" });
            }

            if (!string.IsNullOrEmpty(ProveedorDescripcion) && ProveedorId == 0 && TipoAjuste == TipoStockBines.Productor)
            {
                yield return new ValidationResult(string.Format(Textos.Campo_Invalido, Textos.Productor), new[] { "ProveedorDescripcion" });
            }

            if (!string.IsNullOrEmpty(VinedoPropioDescripcion) && VinedoPropioId == 0 && TipoAjuste == TipoStockBines.VinedoPropio)
            {
                yield return new ValidationResult(string.Format(Textos.Campo_Invalido, Textos.Filtrar_VinedoPropio), new[] { "VinedoPropioDescripcion" });
            }

            if ((!string.IsNullOrEmpty(CentroDescripcion) && CentroId != 0) || (!string.IsNullOrEmpty(ProveedorDescripcion) && ProveedorId != 0) || (!string.IsNullOrEmpty(VinedoPropioDescripcion) && VinedoPropioId != 0))
            {
                yield break;
            }

            var campoRequerido = "CentroDescripcion";
            switch (TipoAjuste)
            {
                case TipoStockBines.Productor:
                    {
                        campoRequerido = "ProveedorDescripcion";
                    }
                    break;
                case TipoStockBines.Centro:
                    {
                        campoRequerido = "CentroDescripcion";
                    }
                    break;
                case TipoStockBines.VinedoPropio:
                    {
                        campoRequerido = "VinedoPropioDescripcion";
                    }
                    break;
            }
            yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.Chofer_Nombre), new[] { campoRequerido });
        }
    }
}
