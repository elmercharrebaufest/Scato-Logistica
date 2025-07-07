using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios.Procesamiento;
using System.Collections.Generic;

public class CategorizadorVehiculo : ICategorizadorVehiculo
{
    private static readonly HashSet<TipoVehiculo> VehiculosEscalables = new HashSet<TipoVehiculo>
    {
        TipoVehiculo.Bitren,
        TipoVehiculo.CamiónC,
        TipoVehiculo.CamiónD,
        TipoVehiculo.CamiónE
    };

    public TipoCategoriaVehiculo ObtenerCategoria(TipoVehiculo tipoVehiculo)
    {
        if (tipoVehiculo == TipoVehiculo.Camión)
            return TipoCategoriaVehiculo.Comun;

        if (VehiculosEscalables.Contains(tipoVehiculo))
            return TipoCategoriaVehiculo.Escalable;

        return TipoCategoriaVehiculo.NoAutomotor;
    }
}