using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;

namespace Molinos.Scato.WebPuertoApi.Helper
{
    public class EmbarqueComparer : IComparer<InstanciaWorkflowPuertoDto>
    {
        public int Compare(InstanciaWorkflowPuertoDto a, InstanciaWorkflowPuertoDto b)
        {
            var fechaA = a.Embarque.ObligacionCarga != null ? a.Embarque.ObligacionCarga : a.Embarque.FechaRecalada;
            var fechaB = b.Embarque.ObligacionCarga != null ? b.Embarque.ObligacionCarga : b.Embarque.FechaRecalada;

            if (fechaA == null && fechaB == null)
                return 0;
            else if (!fechaA.HasValue)
                return 1;
            else if (!fechaB.HasValue)
                return -1;

            if (a.Embarque.Ubicacion == b.Embarque.Ubicacion)
            {
                return fechaA.Value.Ticks > fechaB.Value.Ticks ? 1 : (fechaA.Value.Ticks == fechaB.Value.Ticks ? 0 : -1);
            }
            return a.Embarque.Ubicacion > b.Embarque.Ubicacion ? -1 : 1;

        }
    }
}