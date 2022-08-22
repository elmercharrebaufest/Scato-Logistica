using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEjecutarGrupoBarrera : ProcesadorComando<EjecutarGrupoBarrera>
    {
        private readonly IServicioOrquestador orquestador;

        public ProcesadorEjecutarGrupoBarrera(IRepositorio repositorio, IConversor conversor, ILogger log
            , IServicioOrquestador orquestador)
            : base(repositorio, conversor, log)
        {
            this.orquestador = orquestador;
        }

        public override Resultado Ejecutar(EjecutarGrupoBarrera comando)
        {
            var resultado = new Resultado();
            var estadoSensorArriba = false;
            var estadoSensorAbajo = false;
            var estadoSensorCruce = false;
            var inicioCruce = false;
            var terminoCruce = false;
            var segundundosTranscurridos = 0;
            try
            {
                Log.Info("ProcesadorEjecutatGrupoBarrera Iniciado");
                var grupoBarrera = orquestador.ObtenerConfiguracionGrupoBarrera(comando.Codigo);

                var dispositivoCodigoLista = new List<string>
                {
                    grupoBarrera.SensorAbajo.Codigo,
                    grupoBarrera.SensorArriba.Codigo,
                    grupoBarrera.SensorSegundoCruce.Codigo
                };

                //var dispositivoCodigoLista = new List<string>
                //{
                //    "P6GIROENTRADA",
                //    "P6GIROSALIDA",
                //    "P6FORZADOSALIDA"
                //};

                do
                {
                    System.Threading.Thread.Sleep(1000);

                    segundundosTranscurridos++;

                    Log.Info("ProcesadorEjecutatGrupoBarrera segundos " + segundundosTranscurridos);
                    if (segundundosTranscurridos == 60)
                    {
                        Log.Info("ProcesadorEjecutatGrupoBarrera el procesi fue abortado por que paso 60 segundos");
                        break;
                    }

                    var logEstadoSensor = Repositorio.ListarNoTracking<LogDispositivo>(
                      x => (x.NombreLog == "EstadoSensor" && dispositivoCodigoLista.Contains(x.CodigoDispositivo)));

                    if (logEstadoSensor.Count() < dispositivoCodigoLista.Count)
                    {
                        continue;
                    }

                    var sensorArriba = logEstadoSensor.FirstOrDefault(q => q.CodigoDispositivo == "P6GIROENTRADA");
                    var sensorAbajo = logEstadoSensor.FirstOrDefault(q => q.CodigoDispositivo == "P6GIROSALIDA");
                    var sensorSegundoCruce = logEstadoSensor.FirstOrDefault(q => q.CodigoDispositivo == "P6FORZADOSALIDA");

                    Log.Info("ProcesadorEjecutatGrupoBarrera sensorArriba " + sensorArriba.Valor);

                    Log.Info("ProcesadorEjecutatGrupoBarrera sensorAbajo " + sensorAbajo.Valor);

                    Log.Info("ProcesadorEjecutatGrupoBarrera sensorCruze " + sensorSegundoCruce.Valor);

                    bool.TryParse(sensorArriba.Valor, out estadoSensorArriba);

                    bool.TryParse(sensorAbajo.Valor, out estadoSensorAbajo);

                    bool.TryParse(sensorSegundoCruce.Valor, out estadoSensorCruce);

                    if (estadoSensorArriba == true && estadoSensorAbajo == false)
                    {
                        if (estadoSensorCruce == true && inicioCruce == false)
                            inicioCruce = true;

                        if (inicioCruce == true && estadoSensorCruce == false)
                            terminoCruce = true;
                    }

                    if (inicioCruce = true && estadoSensorAbajo == true && estadoSensorArriba == false)
                        terminoCruce = true;

                    Log.Info("ProcesadorEjecutatGrupoBarrera -------------------------- " + inicioCruce);
                    Log.Info("ProcesadorEjecutatGrupoBarrera inicioCruce " + inicioCruce);
                    Log.Info("ProcesadorEjecutatGrupoBarrera terminoCruce " + terminoCruce);
                    Log.Info("ProcesadorEjecutatGrupoBarrera -------------------------- " + inicioCruce);
                } while (terminoCruce == false);

                if (terminoCruce == true && estadoSensorArriba == true)
                {
                    Log.Info("ProcesadorEjecutatGrupoBarrera Cerrar Barrera " + grupoBarrera.BarreraAbajo.Codigo);
                    var resultadoEjecutarCierreBarrera = orquestador.Ejecutar(new EjecutarCierreBarrera
                    {
                        CodigoDispositivo = grupoBarrera.BarreraAbajo.Codigo
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error("ProcesadorEjecutatGrupoBarrera error " + ex.Message);
                resultado.Error("", Textos.Observacion_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}