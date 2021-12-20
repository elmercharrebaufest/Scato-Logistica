using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Enums
{
    public enum TipoPinchazo : int
    {
        [Description("1 y 2 Pinchazos por Camion/Acoplado")]
        UnoYDos = 1,
        [Description("2 y 3 Pinchazos por Camion/Acoplado")]
        DosYTres = 2,
    }
}
