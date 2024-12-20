using Molinos.Scato.Repositorio;

namespace Molinos.Scato.Servicios.Estrategias
{
    public interface IBalanzadaContext
    {
        IBalanzadaStrategy GetStrategy(string tipoBalanzada);
    }
}