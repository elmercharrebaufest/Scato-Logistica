using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarConfiguracionSensores : ProcesadorModificar<ModificarConfiguracionSensores>
    {
        public ProcesadorModificarConfiguracionSensores(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarConfiguracionSensores comando)
        {
            var config = Repositorio.Obtener<ConfigSensores>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, config);
            config.Descripcion = comando.Dto.Descripcion;
            config.SensorBarreraEntradaArriba = comando.Dto.SensorBarreraEntradaArriba;
            config.SensorBarreraEntradaAbajo = comando.Dto.SensorBarreraEntradaAbajo;
            config.SensorPosicionIngreso = comando.Dto.SensorPosicionIngreso;
            config.SensorPosicionSalida = comando.Dto.SensorPosicionSalida;
            config.SensorBarreraSalidaArriba = comando.Dto.SensorBarreraSalidaArriba;
            config.SensorBarreraSalidaAbajo = comando.Dto.SensorBarreraSalidaAbajo;
        }

        protected override void Validar(ModificarConfiguracionSensores comando, Resultado resultado)
        {

        }
    }
}
