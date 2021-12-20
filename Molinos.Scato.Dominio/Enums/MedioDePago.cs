using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Enums
{
    public enum MedioDePago : int
    {
        [Display(ResourceType = typeof(Textos), Name = "Efectivo")]
        Manual = 0,
        [Display(ResourceType = typeof(Textos), Name = "MercadoPago")]
        MercadoPago = 1
    }
}
