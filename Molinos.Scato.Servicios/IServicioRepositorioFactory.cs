
namespace Molinos.Scato.Servicios
{
    public interface IServicioRepositorioFactory
    {
        IServicioRepositorio CrearServicio(string url);
    }
}
