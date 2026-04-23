using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorFinDeActividad : ProcesadorComando<FinDeActividad>
    {
        private readonly IServicioOrquestador orquestador;

        public ProcesadorFinDeActividad(IRepositorio repositorio, IConversor conversor, IServicioOrquestador orquestador, ILogger log)
            : base(repositorio, conversor, log)
        {
            this.orquestador = orquestador;
        }

        public override Resultado Ejecutar(FinDeActividad comando)
        {
            Log.Debug("Actividad {0}, instanceID {1}, puesto de trabajo {2}", comando.Actividad,comando.InstanceId, comando.PuestoDeTrabajoId);
            Log.Debug($"[ANALISIS_BARRERA] {comando.InstanceId} FinDeActividad {comando.Actividad}");
            var resultado = new Resultado();
            var resultadoEjecutar = new ResultadoEjecutar{Mensaje = new Mensaje()};
            try
            {
                var workflowId = Repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == comando.InstanceId, x => x.Workflow.Id);
                var actividadPorBarrera = Repositorio.Obtener<ActividadPorDispositivo>(
                            x => x.Workflow.Id == workflowId && (x.Salida != null || x.VideoCamaras.Any())
                                          && x.Actividad == comando.Actividad && x.PuestoDeTrabajo.Id == comando.PuestoDeTrabajoId);
                if (actividadPorBarrera == null)
                {
                    Log.Debug("Actividad {0}, instanceID {1}, puesto de trabajo {2}, actividad por dispositivo no encontrada", comando.Actividad, comando.InstanceId, comando.PuestoDeTrabajoId);
                    return resultado;
                }
                foreach (var salida in actividadPorBarrera.Salidas())
                {
                    if (!string.IsNullOrEmpty(salida))
                    {
                        Log.Debug("Ejecutando apertura barrera de salida {0}", salida);
                        resultadoEjecutar = orquestador.Ejecutar(new EjecutarAperturaBarrera { CodigoDispositivo = salida });
                        Log.Debug("Resultado apertura barrera: {0} - {1}", resultadoEjecutar.Mensaje.Codigo, resultadoEjecutar.Mensaje.Descripcion);
                    }
                }

                foreach (var entrada in actividadPorBarrera.Entradas())
                {
                    if (!string.IsNullOrEmpty(entrada))
                    {
                        Log.Debug("Ejecutando cierre barrera de entrada {0}", entrada);
                        resultadoEjecutar = orquestador.Ejecutar(new EjecutarCierreBarrera { CodigoDispositivo = entrada });
                        Log.Debug("Resultado cierre barrera: {0} - {1}", resultadoEjecutar.Mensaje.Codigo, resultadoEjecutar.Mensaje.Descripcion);
                    }
                }


                if (actividadPorBarrera.VideoCamaras != null && actividadPorBarrera.VideoCamaras.Any())
                {
                    var date = DateTime.Now;
                    foreach (var videoCamara in actividadPorBarrera.VideoCamaras)
                    {
                        Log.Debug("Tomando foto {0} en el directorio {1}", videoCamara.Codigo, videoCamara.Directorio);

                        var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                        var fileName = FotoCamionHelper.GenerarNombre(recorrido.Centro.CodigoSAP, recorrido.NumeroDocumentoIngreso, recorrido.Patente, comando.Actividad, date, recorrido.TipoVehiculo);
                        date = date.AddSeconds(1);
                        resultadoEjecutar = orquestador.Ejecutar(new EjecutarTomarFoto { CodigoDispositivo = videoCamara.Codigo, FilePath = videoCamara.Directorio, SubPath = DateTime.Today.ToString("yyyyMMdd"), FileName = fileName });
                        Log.Debug("Resultado foto: {0} - {1}", resultadoEjecutar.Mensaje.Codigo, resultadoEjecutar.Mensaje.Descripcion);   
                    }
                }
            }
            catch (Exception e)
            {
                resultado.Error("Error", Textos.FinDeActividad_ErrorConexionOrquestador);
                Log.Error(e, "Error al procesar el fin de actividad {0}", comando.Actividad);
            }
            if (resultadoEjecutar.Mensaje.Codigo != 0)
            {
                Log.Debug("Error codigo {0}", resultadoEjecutar.Mensaje.Codigo);
                resultado.Error("", resultadoEjecutar.Mensaje.Descripcion); 
            }
            return resultado;
        }
    }
}
