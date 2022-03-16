using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Transactions;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearVisualizacionBarrera : ProcesadorComando<CrearVisualizacionBarrera>
    {
        private readonly IServicioOrquestador orquestador;
        private readonly IConfiguracionProvider config;

        public ProcesadorCrearVisualizacionBarrera(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador orquestador, IConfiguracionProvider config)
            : base(repositorio, conversor, log)
        {
            this.orquestador = orquestador;
            this.config = config;
        }

        public override Resultado Ejecutar(CrearVisualizacionBarrera comando)
        {
            var resultado = new ResultadoCrear();
            var visualizacionBarrera = new VisualizacionBarrera();

            using (var transaction = new TransactionScope())
            {
                try
                {
                    visualizacionBarrera = CrearVisualizacionBarreraSensores(comando);
                    Validar(comando, resultado);
                    if (!resultado.HayErrores)
                    {
                        Repositorio.Agregar(visualizacionBarrera);
                        Repositorio.GuardarCambios();

                        resultado.Id = visualizacionBarrera.Id;

                        var logueaEntidad = comando.GetType().GetCustomAttributes(true).Any(s => s.GetType() == typeof(LoguearEntidad));
                        if (logueaEntidad)
                        {
                            try
                            {
                                var logAbm = new LogABM
                                {
                                    Pantalla = comando.GetType().Name,
                                    Usuario = comando.Usuario,
                                    Fecha = DateTime.Now,
                                    Evento = EventoABM.Alta,
                                    Entidad = comando.ToXml()
                                };
                                Repositorio.Agregar(logAbm);
                                Repositorio.GuardarCambios();
                            }
                            catch (Exception e)
                            {
                                Log.Warn(e, "Ocurrio un error al crear el log AMB Crear");
                            }
                        }

                        transaction.Complete();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error al crear característica de calidad {0}", comando.Dto.Descripcion);
                    resultado.Error("", Textos.Error);
                }

                if (!resultado.HayErrores)
                {
                    foreach (var sensor in comando.Dto.SensoresBarreras)
                    {
                        Suscribir(sensor.CodigoDispositivoSensorArriba);
                        Suscribir(sensor.CodigoDispositivoSensorAbajo);
                    }
                }
            }

            return resultado;
        }

        private void Validar(CrearVisualizacionBarrera comando, Resultado resultado)
        {
            if(string.IsNullOrEmpty(comando.Dto.Codigo))
            {
                resultado.Error("Codigo", Textos.Barrera_crear_codigo_Obligatorio);
            }

            if (string.IsNullOrEmpty(comando.Dto.Descripcion))
            {
                resultado.Error("Descripcion", Textos.Barrera_crear_descripcion_Obligatorio);
            }

            if(comando.Dto.RolId == 0)
            {
                resultado.Error("Descripcion", Textos.Barrera_crear_Rol_Obligatorio);
            }
        }

        private VisualizacionBarrera CrearVisualizacionBarreraSensores(CrearVisualizacionBarrera comando)
        {
            var item = Conversor.Convertir<VisualizacionBarreraDto, VisualizacionBarrera>(comando.Dto);
            if (comando.Dto.SensoresBarreras != null)
            {
                var sensoresBar = (from sensores in comando.Dto.SensoresBarreras
                                  where !sensores._destroy
                                  select Conversor.Convertir<SensorBarreraDto, SensorBarrera>(sensores)).ToList();              
                item.SensoresBarreras = sensoresBar;
            }
            item.Rol = Repositorio.Obtener<Rol>(comando.Dto.RolId);

            return item;
        }

        private void Suscribir(string codigoDispositivo)
        {
            try
            {
                orquestador.Suscribir(new ComandoSuscribir
                {
                    CodigoDispositivo = codigoDispositivo,
                    CodigoEvento = "CambioEstadoSensorBarrera",
                    RutaAccesoSuscriptor = config.AppSettings["UrlNotificacionesWeb"],
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
