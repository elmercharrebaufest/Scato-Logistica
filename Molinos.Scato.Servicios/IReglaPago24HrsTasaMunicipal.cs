using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios
{
    public interface IReglaPago24HrsTasaMunicipal
    {
        bool Aplica(DatosExcepcionTasaMunicipal datos);
        Dictionary<TipoValidacionPagoTasaMunicipal,bool> ValidarExcepcion(DatosExcepcionTasaMunicipal datos);
    }
}
