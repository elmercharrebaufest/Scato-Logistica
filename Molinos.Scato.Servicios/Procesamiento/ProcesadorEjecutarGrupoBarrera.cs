using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEjecutarGrupoBarrera : ProcesadorComando<EjecutarGrupoBarrera>
    {
        private readonly IServicioOrquestador orquestador;

        public ProcesadorEjecutarGrupoBarrera(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador orquestador)
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
            try
            {
                var grupoBarrera = orquestador.ObtenerConfiguracionGrupoBarrera(comando.Codigo);
                var segundundosTranscurridos = 0;

                var resultadoEjecutarAperturaBarrera = orquestador.Ejecutar(new EjecutarAperturaBarrera
                {
                    CodigoDispositivo = grupoBarrera.BarreraArriba.Codigo
                });

                if (resultadoEjecutarAperturaBarrera.Mensaje.Codigo != 0)
                {
                    Log.Error("Fallo la Apertura del dispositivo: {0}", resultadoEjecutarAperturaBarrera.Mensaje.Descripcion);
                }
                else
                {
                    do
                    {
                        //var estadoSensorSegundoCruce = orquestador.Ejecutar(new EjecutarConsultaSensor { CodigoDispositivo = grupoBarrera.SensorSegundoCruce.Codigo }) as ResultadoEstadoSensor;
                        //var estadoSensorAbajo = orquestador.Ejecutar(new EjecutarConsultaSensor { CodigoDispositivo = grupoBarrera.SensorAbajo.Codigo }) as ResultadoEstadoSensor;

                        //if (estadoSensorAbajo.Mensaje) {
                        //    barreraAbierta = false;
                        //}

                        //if (estadoSensorSegundoCruce.EstadoActivo) {
                        //    tocoLaPuntaDelCamion = true;
                        //}
                        //else if (!estadoSensorSegundoCruce.EstadoActivo && tocoLaPuntaDelCamion)
                        //{
                        //    var resultadoEjecutarCierreBarrera = orquestador.Ejecutar(new EjecutarCierreBarrera
                        //    {
                        //        CodigoDispositivo = grupoBarrera.BarreraArriba.Codigo
                        //    });
                        //    camionPasoTramo = true;
                        //}


                        if (estadoSensorArriba == true && estadoSensorAbajo == false)
                        {
                            if (estadoSensorCruce == true && inicioCruce == false)
                                inicioCruce = true;

                            if (inicioCruce == true && estadoSensorCruce == false)
                                terminoCruce = true;
                        }

                        if (inicioCruce = true && estadoSensorAbajo == true && estadoSensorArriba == false)
                            terminoCruce = true;

                        if (segundundosTranscurridos == 60)
                            terminoCruce = true;

                        System.Threading.Thread.Sleep(1000);
                        segundundosTranscurridos++;

                    } while (terminoCruce == false);


                    if (terminoCruce == true && estadoSensorArriba == true)
                    {
                        var resultadoEjecutarCierreBarrera = orquestador.Ejecutar(new EjecutarCierreBarrera
                        {
                            CodigoDispositivo = grupoBarrera.BarreraArriba.Codigo
                        });
                    }
                }


            }
            catch (Exception)
            {
                resultado.Error("", Textos.Observacion_ErrorEnLaCarga);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }
    }
}