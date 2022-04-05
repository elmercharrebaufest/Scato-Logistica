using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarVisualizacionBarrera : ProcesadorModificar<ModificarVisualizacionBarrera>
    {
        private readonly IServicioOrquestador orquestador;
        private readonly IConfiguracionProvider config;

        public ProcesadorModificarVisualizacionBarrera(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador orquestador, IConfiguracionProvider config)
            : base(repositorio, conversor, log)
        {
            this.orquestador = orquestador;
            this.config = config;
        }

        protected override void ModificarEntidad(ModificarVisualizacionBarrera comando)
        {
            var visualizacionBarrera = Repositorio.Obtener<VisualizacionBarrera>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, visualizacionBarrera);

            if(visualizacionBarrera.Rol.Id != comando.Dto.RolId)
            {
                visualizacionBarrera.Rol = Repositorio.Obtener<Rol>(comando.Dto.RolId);
            }

            if (comando.SensoresBorrados != null)
            {
                foreach (var idBorrado in comando.SensoresBorrados)
                {
                    Repositorio.Remover<SensorBarrera>(idBorrado);
                }
            }

            if (comando.Dto.SensoresBarreras != null)
            {
                var sensores = (from sensor in comando.Dto.SensoresBarreras
                                  where !sensor._destroy
                                  select Conversor.Convertir<SensorBarreraDto, SensorBarrera>(sensor)).ToList();
                foreach (var sensor in sensores)
                {
                    visualizacionBarrera.SensoresBarreras.Add(sensor);
                    Suscribir(sensor.CodigoDispositivoSensorArriba);
                    Suscribir(sensor.CodigoDispositivoSensorAbajo);
                }
            }

            foreach (var sensor in visualizacionBarrera.SensoresBarreras)
            {
                Suscribir(sensor.CodigoDispositivoSensorArriba);
                Suscribir(sensor.CodigoDispositivoSensorAbajo);
            }
        }

        protected override void Validar(ModificarVisualizacionBarrera comando, Resultado resultado)
        {

        }

        private void Suscribir(string codigoDispositivo)
        {
            try
            {
                orquestador.Suscribir(new ComandoSuscribir
                {
                    CodigoDispositivo = codigoDispositivo,
                    CodigoEvento = "CambioEstadoSensor",
                    RutaAccesoSuscriptor = config.AppSettings["UrlNotificaciones"],
                    Persistente = true
                });
            }
            catch (Exception e)
            {
                Log.Error($"Error al suscribir sensor de barrera: {codigoDispositivo}", e);
            }
        }
    }
}
