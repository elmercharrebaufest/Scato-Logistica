using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class EficienciaCaladoController : ConsultasController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;
        IConfiguracionProvider configuracion;

        public EficienciaCaladoController(
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
            return View();
        }

        public JsonResult ObtenerEficiencia()
        {
            var centroId = 5;
            var configuracionDeCalles = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EficienciaCalado, Constantes.ConfiguracionGeneral.EficienciaCalado.EficienciaCalles, centroId);
            var configuracionEficienciaCalles = configuracionDeCalles.Valor.FromJson<List<EficienciaCaladoValoresDto>>();

            var listaEficienciaCalado = new List<EficienciaCaladoDatosGraficosDto>
            {
                ObtenerEficienciaPorHora(configuracionEficienciaCalles),
                ObtenerEficienciaPorTurno(configuracionEficienciaCalles)
            };
            return Json(listaEficienciaCalado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCamionesPorCalar()
        {
            var pendientesPorCalar = servicio.ObtenerCantidadPendientesPorCalar(5);
            return Json(pendientesPorCalar, JsonRequestBehavior.AllowGet);
        }

        private EficienciaCaladoDatosGraficosDto ObtenerEficienciaPorHora(List<EficienciaCaladoValoresDto> configuraciones)
        {
            var resultado = new EficienciaCaladoDatosGraficosDto();
            try
            {
                var hasta = DateTime.Now;
                var desde = hasta.AddHours(-1);
                var listaEficienciaValores = servicio.ObtenerEficienciaCalado(desde, hasta);

                foreach (var eficienciaTeorica in configuraciones)
                {
                    var caladoPorCalle = listaEficienciaValores.Where(c => c.Id == eficienciaTeorica.Id).FirstOrDefault();
                    if(caladoPorCalle != null)
                    {
                        decimal porcentaje = (decimal)(caladoPorCalle.Cantidad * 100) / eficienciaTeorica.Cantidad;
                        resultado.Porcentajes.Add(decimal.Round(porcentaje, 2));
                        resultado.Calles.Add(eficienciaTeorica.Nombre);
                    } else
                    {
                        resultado.Porcentajes.Add(0);
                        resultado.Calles.Add(eficienciaTeorica.Nombre);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return resultado;
        }

        private EficienciaCaladoDatosGraficosDto ObtenerEficienciaPorTurno(List<EficienciaCaladoValoresDto> configuraciones)
        {
            var resultado = new EficienciaCaladoDatosGraficosDto();
            try
            {
                var configuracionHorario = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EficienciaCalado, Constantes.ConfiguracionGeneral.EficienciaCalado.HorarioTurno, 5);
                var horario = configuracionHorario.Valor.FromJson<ConfiguracionHorarioDto>();
                var configuracionHoraEntrada = TimeSpan.Parse(horario.HoraEntrada);
                var configuracionHoraSalida = TimeSpan.Parse(horario.HoraSalida);
                var cantidadDeHoras = (configuracionHoraSalida - configuracionHoraEntrada).TotalHours;
                var hoy = DateTime.Now;
                DateTime hasta = hoy, desde = hoy;

                if(hoy.Hour >= 00 && hoy.Hour < configuracionHoraEntrada.Hours)
                {
                    var fecha = hoy.AddDays(-1);
                    desde = fecha.Date.Add(configuracionHoraSalida);
                    hasta = hoy.Date.Add(configuracionHoraEntrada);
                } else if((hoy.Hour >= configuracionHoraSalida.Hours && hoy.Minute > 0 ) && hoy.Hour < 24)
                {
                    var fecha = hoy.AddDays(1);
                    desde = hoy.Date.Add(configuracionHoraSalida);
                    hasta = fecha.Date.Add(configuracionHoraEntrada);
                } else
                {
                    desde = hoy.Date.Add(configuracionHoraEntrada);
                    hasta = hoy.Date.Add(configuracionHoraSalida);
                }

                var listaEficienciaValores = servicio.ObtenerEficienciaCalado(desde, hasta);
                foreach (var eficienciaTeorica in configuraciones)
                {
                    var caladoPorCalle = listaEficienciaValores.Where(c => c.Id == eficienciaTeorica.Id).FirstOrDefault();
                    if (caladoPorCalle != null)
                    {
                        decimal porcentaje = (caladoPorCalle.Cantidad * 100) / (eficienciaTeorica.Cantidad * (decimal)cantidadDeHoras);
                        resultado.Porcentajes.Add(decimal.Round(porcentaje, 2));
                        resultado.Calles.Add(eficienciaTeorica.Nombre);
                    }
                    else
                    {
                        resultado.Porcentajes.Add(0);
                        resultado.Calles.Add(eficienciaTeorica.Nombre);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return resultado;
        }
    }
}
