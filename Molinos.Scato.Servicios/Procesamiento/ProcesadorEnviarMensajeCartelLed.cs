using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEnviarMensajeCartelLed : ProcesadorComando<EnviarMensajeCartelLed>
    {
        private readonly IServicioOrquestador orquestador;

        public ProcesadorEnviarMensajeCartelLed(IRepositorio repositorio, IConversor conversor, IServicioOrquestador orquestador, ILogger log)
            : base(repositorio, conversor, log)
        {
            this.orquestador = orquestador;
        }

        public override Resultado Ejecutar(EnviarMensajeCartelLed comando)
        {
            if (comando.SegundosDeEspera > 0)
                Task.Delay(TimeSpan.FromSeconds(comando.SegundosDeEspera)).Wait();

            var resultadoComando = new Resultado();
            Log.Debug($"Mensaje {comando.Mensaje}, Codigo {comando.Codigo} puesto {comando.PuestoDeTrabajoId}");
            if (string.IsNullOrEmpty(comando.Codigo) && comando.PuestoDeTrabajoId != 0)
            {
                comando.Codigo = Repositorio.ObtenerProyeccion<PuestoDeTrabajo, string>(x => x.Id == comando.PuestoDeTrabajoId, x => x.CartelLed);
            }
            if (!string.IsNullOrEmpty(comando.Codigo))
            {
                ResultadoEjecutar resultado;
                if (!comando.EsMensajeConIntervalo)
                {
                    resultado = orquestador.Ejecutar(new EjecutarEnviarMensaje
                    {
                        Texto = comando.Mensaje,
                        CodigoDispositivo = comando.Codigo,
                        NumeroTrama = comando.NumeroTrama,
                        NumeroPrograma = comando.NumeroPrograma,
                        NumeroVariable = comando.NumeroVariable
                    });
                }
                else
                {
                    resultado = orquestador.Ejecutar(new EjecutarEnviarMensajeIntervalo
                    {
                        Texto = comando.Mensaje,
                        TextSecundario = comando.MensajeSecundario,
                        CodigoDispositivo = comando.Codigo,
                        NumeroTrama = comando.NumeroTrama,
                        NumeroPrograma = comando.NumeroPrograma,
                        NumeroVariable = comando.NumeroVariable,
                        Intervalo = comando.IntervaloMilliseconds ?? 3000
                    });
                }

                if (resultado.Mensaje != null && resultado.Mensaje.Codigo != 0)
                {
                    Log.Error($"Error al enviar EnviarMensajeCarteLed{comando.Mensaje},{comando.Codigo} al orquestador" + resultado.Mensaje.Descripcion);
                    resultadoComando.Errores.Add("comando.Codigo", resultado.Mensaje.Descripcion);
                }
            }

            return resultadoComando;
        }
    }
}