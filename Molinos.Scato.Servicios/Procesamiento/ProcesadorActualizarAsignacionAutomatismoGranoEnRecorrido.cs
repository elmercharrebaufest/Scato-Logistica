using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarAsignacionAutomatismoGranoEnRecorrido : ProcesadorModificar<ActualizarAsignacionAutomatismoGranoEnRecorrido>
    {
        public ProcesadorActualizarAsignacionAutomatismoGranoEnRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ActualizarAsignacionAutomatismoGranoEnRecorrido comando)
        {
            var asignacion = Repositorio.Obtener<AsignacionAutomatismoGranoEnRecorrido>(x => x.RecorridoId == comando.RecorridoId);

            asignacion.CallePreHidraulicaId = comando.CallePreHidraulicaId;
            asignacion.CallePreBalanzaId = comando.CallePreBalanzaId;
        }

        protected override void Validar(ActualizarAsignacionAutomatismoGranoEnRecorrido comando, Resultado resultado)
        {
            if (!Repositorio.Existe<AsignacionAutomatismoGranoEnRecorrido>(x => x.RecorridoId == comando.RecorridoId))
            {
                resultado.Error("AsignacionAutomatismoGranoEnRecorrido", Textos.AsignacionAutomatismo_Inexistente);
            }
        }
    }
}