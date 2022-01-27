using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEnviarMensajeAsincronoCartelLed : ProcesadorComando<EnviarMensajeAsincronoCartelLed>
    {
        private readonly IServicioOrquestador orquestador;
        private readonly IKernel kernel;
        private readonly IServicioComandos servicioComandos;

        public ProcesadorEnviarMensajeAsincronoCartelLed(IRepositorio repositorio, IConversor conversor, IServicioOrquestador orquestador, ILogger log, IKernel kernel, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.orquestador = orquestador;
            this.kernel = kernel;
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(EnviarMensajeAsincronoCartelLed comando)
        {
            var resultadoComando = new Resultado();

            try
            {
                comando.Codigo = Repositorio.ObtenerProyeccion<PuestoDeTrabajo, string>(x => x.Id == comando.PuestoDeTrabajoId, x => x.CartelLed);
                if (!string.IsNullOrEmpty(comando.Codigo))
                {
                    AsynchronousEnviarMensajeCartelLEd(comando);
                } 
                else
                {
                    Log.Debug($"No se encontro dispositivo cartelLed par el puesto de trabajo : {comando.PuestoDeTrabajoId}");
                }
                
            }
            catch (Exception e)
            {
                Log.Error(e, "Error EnviarMensajeCartelLed Task AsyncEnviarMensaje");
            }

            return resultadoComando;
        }

        private async void AsynchronousEnviarMensajeCartelLEd(EnviarMensajeAsincronoCartelLed comando)
        {
            await AsyncEnviarMensaje(comando);
        }

        public async Task AsyncEnviarMensaje(EnviarMensajeAsincronoCartelLed mensaje)
        {
            try
            {
                if (mensaje.SegundosDeEspera > 0)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(mensaje.SegundosDeEspera));
                        CrearLogActividad(mensaje.WorkflowInstanceId);
                        EnviarMensaje(mensaje);
                    });
                }
                else
                {
                    CrearLogActividad(mensaje.WorkflowInstanceId);
                    EnviarMensaje(mensaje);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error EnviarMensajeCartelLed Task AsyncEnviarMensajes");
            }
        }

        private void EnviarMensaje(EnviarMensajeAsincronoCartelLed comando)
        {
            Log.Debug($"Mensaje {comando.Mensaje}, Codigo {comando.Codigo} puesto {comando.PuestoDeTrabajoId} segundosEspera {comando.SegundosDeEspera}");
            using (var channelFactory = new ChannelFactory<IServicioOrquestador>("Orquestador"))
            {
                IServicioOrquestador channel = channelFactory.CreateChannel();
                var resultado = channel.Ejecutar(new EjecutarEnviarMensaje { Texto = comando.Mensaje, CodigoDispositivo = comando.Codigo, NumeroTrama = comando.NumeroTrama, NumeroPrograma = comando.NumeroPrograma, NumeroVariable = comando.NumeroVariable });
                if (resultado.Mensaje != null && resultado.Mensaje.Codigo != 0)
                {
                    Log.Error($"Error al enviar EnviarMensajeCarteLed {comando.Mensaje},{comando.Codigo} al orquestador" + resultado.Mensaje.Descripcion);
                    CrearControlRecorrido(comando.WorkflowInstanceId, resultado.Mensaje.Descripcion);
                }
                else
                {
                    Log.Debug($"Se envio el mensaje correctamente. |{comando.Mensaje}| Codigo {comando.Codigo} puesto {comando.PuestoDeTrabajoId} segundosEspera {comando.SegundosDeEspera}");
                    CrearControlRecorrido(comando.WorkflowInstanceId, "ok");
                }
            }
        }

        private void CrearLogActividad(Guid workflowInstanceId)
        {
            try
            {
                kernel.Get<IServicioComandos>().Ejecutar(new CrearLogActividad
                {
                    Dto = new LogActividadDto
                    {
                        Actividad = "EnviarMensaje",
                        ActividadXaml = "EnviarMensaje",
                        WorkflowInstanceId = workflowInstanceId,
                        Fecha = DateTime.Now
                    }
                });
                
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrio un error al CrearLogActividad WorkflowInstanceId: {0}", workflowInstanceId);
            }
        }

        private void CrearControlRecorrido(Guid workflowInstanceId, string comentario)
        {
            try
            {
                kernel.Get<IServicioComandos>().Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "EnviarMensaje",
                        Fecha = DateTime.Now,
                        Comentario = comentario,
                        NombreUsuario = "",
                        WorkflowInstanceId = workflowInstanceId,
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrio un error al CrearLogActividad WorkflowInstanceId: {0}", workflowInstanceId);
            }
        }
    }
}
