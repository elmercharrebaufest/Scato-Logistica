using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public interface ICategorizadorVehiculo
    {
        TipoCategoriaVehiculo ObtenerCategoria(TipoVehiculo tipoVehiculo);
    }
}
