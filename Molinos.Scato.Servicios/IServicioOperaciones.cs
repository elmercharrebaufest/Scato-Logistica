using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios
{
    public interface IServicioOperaciones
    {
        /// <summary>
        /// ObtenerOrdenesDeCarga
        /// </summary>
        /// <param name="patente"></param>
        /// <exception cref="FileNotFoundException">Why it's thrown.</exception>
        /// <returns></returns>
        IEnumerable<OrdenDeCargaDto> ObtenerOrdenesDeCarga(string patente);

        /// <summary>
        /// InformarViaje
        /// </summary>
        void InformarViajeOrdenesDeCargaFason(IngresosEgresosFasonesDto ingresosEgresosFasonesDto);
    }
   
}
