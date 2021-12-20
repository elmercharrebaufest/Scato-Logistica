using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AjusteDeCalidadDto
    {
        public int Id { get; set; }
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? ValorOriginal { get; set; }
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? ValorNuevo { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Material { get; set; }
        public string Caracteristica { get; set; }
        public int CaracteristicaId { get; set; }
        public string Rango { get; set; }
        public bool EsModificable { get; set; }
        public bool EsAnalisis { get; set; }
    }
}
