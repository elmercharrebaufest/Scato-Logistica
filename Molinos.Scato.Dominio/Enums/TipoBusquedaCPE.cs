using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Enums
{

    /// <summary>
    /// Tipos de búsqueda válidos para Cartas de Porte Electrónicas.
    /// </summary>
    public enum TipoBusquedaCPE
    {
        /// <summary>Búsqueda en caché por patente del vehículo</summary>
        BusquedaPorPatente = 1,

        /// <summary>Consulta directa a AFIP sin usar caché</summary>
        ConsultaDirectaAfip = 2,

        /// <summary>Búsqueda en caché con fallback a AFIP si no se encuentra</summary>
        CacheConFallbackAfip = 3
    }
}
