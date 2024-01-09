using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarAutomatismoGranos : ProcesadorEliminar<EliminarAutomatismoGranos, AutomatismoGrano>
    {
        public ProcesadorEliminarAutomatismoGranos(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarAutomatismoGranos comando)
        {
            return comando.IdAutomatismo;
        }

        protected override void Validar(EliminarAutomatismoGranos comando, Resultado resultado)
        {
            if (!Repositorio.Existe<AutomatismoGrano>(a => a.Id == comando.IdAutomatismo))
            {
                resultado.Error("Id Automatismo", Textos.Automatismo_IdExistente);
            }
        }
    }
}