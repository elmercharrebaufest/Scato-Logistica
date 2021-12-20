using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearReasignacionDeTarjeta : ProcesadorCrear<CrearReasignacionDeTarjeta, ReasignacionDeTarjeta>
    {
        public ProcesadorCrearReasignacionDeTarjeta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ReasignacionDeTarjeta CrearEntidad(CrearReasignacionDeTarjeta comando)
        {
            var entidad = Conversor.Convertir<ReasignacionDeTarjetaDto, ReasignacionDeTarjeta>(comando.Dto);
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.InstanceId);
            var actividad = Repositorio.ObtenerMayor<LogActividad, int>(y => y.WorkflowInstanceId == comando.Dto.InstanceId, x => x.Id);
            entidad.Patente = recorrido != null ? recorrido.Patente : null;
            entidad.Etapa = actividad != null ? actividad.Actividad : null;

            return entidad;
        }

        protected override void Validar(CrearReasignacionDeTarjeta comando, Resultado resultado)
        {

        }
    }
}
