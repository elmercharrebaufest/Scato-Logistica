using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Molinos.Scato.Dominio.Enums
{
    public enum SectorEnum
    {
        [Description("Mesa de Entrada")]
        MesaDeEntrada = 0,

        [Description("Calado")]
        Calado = 1,

        [Description("Playa Externa")]
        PlayaExterna = 2,

        [Description("En Transito")]
        EnTransito = 3,

        [Description("Pesada Bruto")]
        PesadaBruto = 4,

        [Description("Carga Descarga")]
        CargaDescarga = 5
    }
}
