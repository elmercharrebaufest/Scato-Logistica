using System.Data.Entity;

namespace Molinos.Scato.Repositorio
{
    public interface IComando<TResultado>
    {
        TResultado Ejecutar(DbContext contexto);
    }
}
