using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarAsignacionAutomatismoNoGranoEnRecorrido : ProcesadorModificar<ActualizarAsignacionNoGranoEnRecorrido>
    {
        public ProcesadorActualizarAsignacionAutomatismoNoGranoEnRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ActualizarAsignacionNoGranoEnRecorrido comando)
        {
            var asignacion = Repositorio.Obtener<AsignacionNoGranoEnRecorrido>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, asignacion);
        }

        protected override void Validar(ActualizarAsignacionNoGranoEnRecorrido comando, Resultado resultado)
        {
            
        }
    }
}