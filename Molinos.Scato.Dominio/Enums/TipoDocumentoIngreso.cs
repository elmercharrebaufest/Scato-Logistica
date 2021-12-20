using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    //IMPORTANTE: Cada vez que se agregue un dato se debe actualizar manualmente en la tabla correspondiente.
    //[DataContract(Name = "tipoDocumentoIngreso")]
    public enum TipoDocumentoIngreso
    {
        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna")]
       // [EnumMember]
        OrdenCargaInterna,
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte")]
      //  [EnumMember]
        CartaPorte,
        [Display(ResourceType = typeof(Textos), Name = "OrdenDeDescarga")]
      //  [EnumMember]
        OrdenDeDescarga,
        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaFAS")]
        //  [EnumMember]
        OrdenCargaFas,
        [Display(ResourceType = typeof(Textos), Name = "OrdenDeDescargaFason")]
      //  [EnumMember]
        OrdenDeDescargaFason,
        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInternaFason")]
      //  [EnumMember]
        OrdenCargaInternaFason,
        [Display(ResourceType = typeof(Textos), Name = "OrdenEntrePlantas")]
        //  [EnumMember]
        OrdenEntrePlantas,
       [Display(ResourceType = typeof(Textos), Name = "IngresarRemito")]
      //  [EnumMember]
        Remito,
        [Display(ResourceType = typeof(Textos), Name = "OrdenDeCargaContenedor")]
      //  [EnumMember]
        OrdenDeCargaContenedor,
        [Display(ResourceType = typeof(Textos), Name = "RemitoBodegaUvaPropia")]
      //  [EnumMember]
        RemitoBodegaUvaPropia,
        [Display(ResourceType = typeof(Textos), Name = "RemitoBodegaUvaTerceros")]
      //  [EnumMember]
        RemitoBodegaUvaTerceros,
        [Display(ResourceType = typeof(Textos), Name = "RemitoBodegaVinoTerceros")]
      //  [EnumMember]
        RemitoBodegaVino,
        [Display(ResourceType = typeof(Textos), Name = "HojaDeRuta")]
      //  [EnumMember]
        HojaDeRuta,
        [Display(ResourceType = typeof(Textos), Name = "HojaDeRutaYerbatera")]
      //  [EnumMember]
        HojaDeRutaYerbatera,
        [Display(ResourceType = typeof(Textos), Name = "Embarque")]
        //  [EnumMember]
        Embarque
    }
}
