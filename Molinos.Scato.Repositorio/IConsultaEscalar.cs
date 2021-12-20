using System.Data.Entity;

namespace Molinos.Scato.Repositorio
{
    public interface IConsultaEscalar<TEntidad>
    {
       TEntidad Ejecutar(DbContext contexto);
    }
}
