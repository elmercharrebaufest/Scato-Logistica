using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEnviarMensajesAsincronoCartelLed : ProcesadorComando<EnviarMensajesAsincronoCartelLed>
    {
        private readonly IServicioOrquestador orquestador;
        private readonly IKernel kernel;

        public ProcesadorEnviarMensajesAsincronoCartelLed(IRepositorio repositorio, IConversor conversor, IServicioOrquestador orquestador, ILogger log, IKernel kernel)
            : base(repositorio, conversor, log)
        {
            this.orquestador = orquestador;
            this.kernel = kernel;
        }
        public override Resultado Ejecutar(EnviarMensajesAsincronoCartelLed comando)
        {
            var resultadoComando = new Resultado();

            try
            {
                for (int i = 0; i < comando.Mensajes.Count; i++)
                {
                    var puestoTrabajo = comando.Mensajes[i].PuestoDeTrabajoId;
                    comando.Mensajes[i].Codigo = Repositorio.ObtenerProyeccion<PuestoDeTrabajo, string>(x => x.Id == puestoTrabajo, x => x.CartelLed);
                    
                    if (string.IsNullOrEmpty(comando.Mensajes[i].Codigo))
                    {
                        comando.Mensajes.RemoveAt(i);
                    }
                }

                AsynchronousEnviarMensajesCartelLEd(comando.Mensajes);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error EnviarMensajeCartelLed Task AsyncEnviarMensajes");
            }

            return resultadoComando;
        }

        private async void AsynchronousEnviarMensajesCartelLEd(List<EnviarMensajeCartelLed> comando)
        {
            await AsyncEnviarMensajes(comando);
        }

        public async Task AsyncEnviarMensajes(List<EnviarMensajeCartelLed> listaMensajes)
        {
            try
            {
                foreach (var mensaje in listaMensajes)
                {
                    if(mensaje.SegundosDeEspera > 0)
                    {
                        await Task.Run(() =>
                        {                            
                            Thread.Sleep(TimeSpan.FromSeconds(mensaje.SegundosDeEspera));
                            EnviarMensaje(mensaje);
                        });
                    }
                    else
                    {
                        EnviarMensaje(mensaje);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error EnviarMensajeCartelLed Task AsyncEnviarMensajes");
            }
        }

        private void EnviarMensaje(EnviarMensajeCartelLed comando)
        {
            Log.Debug($"Mensaje {comando.Mensaje}, Codigo {comando.Codigo} puesto {comando.PuestoDeTrabajoId} segundosEspera {comando.SegundosDeEspera}");
            using (var channelFactory = new ChannelFactory<IServicioOrquestador>("Orquestador"))
            {
                IServicioOrquestador channel = channelFactory.CreateChannel();
                var resultado = channel.Ejecutar(new EjecutarEnviarMensaje { Texto = comando.Mensaje, CodigoDispositivo = comando.Codigo, NumeroTrama = comando.NumeroTrama, NumeroPrograma = comando.NumeroPrograma, NumeroVariable = comando.NumeroVariable });
                if (resultado.Mensaje != null && resultado.Mensaje.Codigo != 0)
                {
                    Log.Error($"Error al enviar EnviarMensajeCarteLed {comando.Mensaje},{comando.Codigo} al orquestador" + resultado.Mensaje.Descripcion);
                }
                else
                {
                    Log.Debug($"Se envio el mensaje correctamente. |{comando.Mensaje}| Codigo {comando.Codigo} puesto {comando.PuestoDeTrabajoId} segundosEspera {comando.SegundosDeEspera}");
                }
            }
        }
    }
}
