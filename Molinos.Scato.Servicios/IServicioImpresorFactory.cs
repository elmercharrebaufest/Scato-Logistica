
using Molinos.Scato.Servicios.ServicioImpresion;

namespace Molinos.Scato.Servicios
{
    public interface IServicioImpresorFactory
    {
        IServicioImpresion CrearServicio();
    }
}
