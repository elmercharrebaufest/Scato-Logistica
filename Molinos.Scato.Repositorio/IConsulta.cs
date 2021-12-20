using System.Collections.Generic;
using System.Data.Entity;

namespace Molinos.Scato.Repositorio
{
    public interface IConsulta<TEntidad>
    {
       List<TEntidad> Ejecutar(DbContext contexto);
    }
}
