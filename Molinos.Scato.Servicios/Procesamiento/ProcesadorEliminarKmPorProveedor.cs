using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarKmPorProveedor : ProcesadorEliminar<EliminarKmPorProveedor, KmPorProveedor>
    {
        public ProcesadorEliminarKmPorProveedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarKmPorProveedor comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarKmPorProveedor comando, Resultado resultado)
        {
        }
    }
}
