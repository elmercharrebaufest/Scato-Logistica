using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios
{
    public interface IReglaTasaMunicipal
    {
        bool Aplica(DatosTasaMunicipal datos);
        TipoVehiculo ObtenerTipoVehiculo(DatosTasaMunicipal datos);
        Dictionary<int, TipoValidacionPagoTasaMunicipal> ObtenerPago(DatosTasaMunicipal datos, TipoCategoriaVehiculo tipoCategoria, int numeroDiasDesde);
    }
}
