using System.Data.Entity;
using Molinos.Scato.Dominio.Consultas;

namespace Molinos.Scato.Repositorio
{
    public interface IConsultaPaginada<TEntidad>
    {
        ListaPaginada<TEntidad> Ejecutar(DbContext contexto);
    }
}
