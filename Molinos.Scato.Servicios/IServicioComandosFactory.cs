
namespace Molinos.Scato.Servicios
{
    public interface IServicioComandosFactory
    {
        IServicioComandos CrearServicio(string url);
    }
}
