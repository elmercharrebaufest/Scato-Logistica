using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    [DataContract]
    [Flags]
    public enum CantidadDeResultados
    {
        [Display(ResourceType = typeof(Textos), Name = "Diez")]
        [EnumMember]
        Diez = 10,
        [Display(ResourceType = typeof(Textos), Name = "Veinticinco")]
        [EnumMember]
        Veinticinco = 25,
        [Display(ResourceType = typeof(Textos), Name = "Cincuenta")]
        [EnumMember]
        Cincuenta = 50,
        [Display(ResourceType = typeof(Textos), Name = "Cien")]
        [EnumMember]
        Cient = 100
    }
}
