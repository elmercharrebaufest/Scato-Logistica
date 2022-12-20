using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class ConfiguracionCartelesFijosPreCaladoController : ConsultasController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;
        IConfiguracionProvider configuracion;

        public ConfiguracionCartelesFijosPreCaladoController(
           ILogger log,
           IServicioRepositorio servicio,
           IConfiguracionProvider configuracion,
           IServicioComandos servicioComandos

           ) : base(log, servicio, configuracion)
        {
            this.log = log;
            this.servicio = servicio;
            this.configuracion = configuracion;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index()
        {
            try
            {
                var cartel = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePreCalado, Constantes.ConfiguracionGeneral.PreCalado.CartelLedCalador);
                var mensajeCartelTitulo = servicio.ObtenerMensajeCartelLedPorCodigo(CodigoMensajeCartelLed.CartelPrecaladoTitulo);
                var mensajeCartelCalador1 = servicio.ObtenerMensajeCartelLedPorCodigo(CodigoMensajeCartelLed.CartelPrecaladoCalleCalador1);
                var mensajeCartelCalador2 = servicio.ObtenerMensajeCartelLedPorCodigo(CodigoMensajeCartelLed.CartelPrecaladoCalleCalador2);
                var callesCalador = servicio.ListarCallesPorTipo(TipoCalle.Calado);

                if (cartel != null)
                {
                    EjecutarEnvioMensajeCartelLed(mensajeCartelTitulo, cartel);
                    EjecutarEnvioMensajeCartelLed(mensajeCartelCalador1, cartel);
                    EjecutarEnvioMensajeCartelLed(mensajeCartelCalador2, cartel);
                }

            }
            catch (Exception e)
            {
                log.Error(e, e.Message);
            }

            return View();
        }

        private void EjecutarEnvioMensajeCartelLed(MensajeCartelLedDto mensajeCartel, ConfiguracionGeneralDto cartel)
        {
            if (mensajeCartel != null)
            {
                servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                {
                    Mensaje = $"{mensajeCartel.Mensaje}",
                    Codigo = cartel.Valor,
                    NumeroPrograma = mensajeCartel.Programa,
                    NumeroTrama = mensajeCartel.Trama,
                    NumeroVariable = mensajeCartel.Variable,
                    SegundosDeEspera = mensajeCartel.SegundosDeEspera
                });
            }
            else
            {
                throw new Exception($"No se pudo mostrar el mensaje {mensajeCartel.Codigo}");
            }
        }

    }
}
