using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    public class SincronizarMaestrosController : Controller
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicioRepositorio;
        private ILogger log;

        public SincronizarMaestrosController(IServicioComandos servicioComandos, IServicioRepositorio servicioRepositorio, ILogger log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioRepositorio = servicioRepositorio;
            this.log = log;
        }

        public ActionResult Proveedores(bool mostrarResultados = true)
        {
            log.Info("Ejecutando sincronización de Proveedores. Mostrar resultados: {0}", mostrarResultados);
            var resultado = servicioComandos.Ejecutar(new SincronizarProveedores { RetornarResultado = mostrarResultados, CargaMasiva = true }) as ResultadoSincronizarProveedores;
            ModelState.AgregarErrores(resultado);
            if (mostrarResultados)
            {
                ModelState.AgregarErrores(resultado);
                return View(resultado);
            }
            return Content(string.Empty);
        }

        public ActionResult Clientes(bool mostrarResultados = true)
        {
            log.Info("Ejecutando sincronización de Clientes. Mostrar resultados: {0}", mostrarResultados);
            var resultado = servicioComandos.Ejecutar(new SincronizarClientes { RetornarResultado = mostrarResultados, CargaMasiva = true }) as ResultadoSincronizarClientes;
            ModelState.AgregarErrores(resultado);
            if (mostrarResultados)
            {
                ModelState.AgregarErrores(resultado);
                return View(resultado);
            }
            return Content(string.Empty);
        }

        public ActionResult Materiales(bool mostrarResultados = true)
        {
            log.Info("Ejecutando sincronización de Materiales. Mostrar resultados: {0}", mostrarResultados);
            var resultado = servicioComandos.Ejecutar(new SincronizarMateriales { RetornarResultado = mostrarResultados, CargaMasiva = true }) as ResultadoSincronizarMateriales;
            if (mostrarResultados)
            {
                ModelState.AgregarErrores(resultado);
                return View(resultado);
            }
            return Content(string.Empty);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult CachearCpeAfip(DatosUsuario datosUsuario)
        {
            CachearCpeAfip(datosUsuario.CentroId, 3, "AMBOS");
            return Json(true);
        }

        public void ActualizarCuposOtorgados()
        {
            var centros = servicioRepositorio.ListarCentros().Select(centro => (centro.CodigoSAP + ',' + centro.CodigoSAPEspecial).Split(',').Where(x => !string.IsNullOrEmpty(x)));
            servicioComandos.Ejecutar(new ActualizarCuposOtorgados { CentrosCodigoSap = centros.SelectMany(x => x).ToArray(), Fecha = DateTime.Now });
        }

        public void ActualizarEstadoPlanta()
        {
            var centros = servicioRepositorio.ListarCentros().Select(centro => (centro.CodigoSAP + ',' + centro.CodigoSAPEspecial).Split(',').Where(x => !string.IsNullOrEmpty(x)));
            servicioComandos.Ejecutar(new ActualizarEstadoMaterial());
        }

        public void ActualizarStock(DateTime? fecha = null, int diasAtras = 0)
        {
            fecha = fecha ?? DateTime.Now;
            var fechaInicio = fecha.Value.AddDays(diasAtras * -1);
            for (var i = 0; fechaInicio <= fecha; i++)
            {
                servicioComandos.Ejecutar(new ActualizarStock() { Fecha = fechaInicio });
                fechaInicio = fechaInicio.AddDays(1);
            }
        }

        public void TransmisionBajaCTG(int centroId = 5)
        {
            log.Info("Ejecutando transmision BajaCTG");
            try
            {
                var bajaCTGconError = servicioRepositorio.ListarBajaCTGError(centroId);
                if (bajaCTGconError.Count() > 0)
                {
                    List<Resultado> resultados = new List<Resultado>();
                    log.Info($"Cantidad de bajas: {bajaCTGconError.Count()}");

                    foreach (var item in bajaCTGconError)
                    {
                        var resultado = new Resultado();
                        if (item.EstadoCtg == EstadoTransmisionASap.Error)
                        {
                            try
                            {
                                if (item.Dto.Cpe)
                                {
                                    log.Info($"Confirmar arribo, CGT: {item.Dto.CTG}, workflowId: {item.WorkflowId}");
                                    resultados.Add(servicioComandos.Ejecutar(new ConfirmarArribo
                                    {
                                        Dto = item.Dto,
                                        Vehiculo = item.Vehiculo,
                                        CentroId = item.CentroId,
                                        WorkflowId = item.WorkflowId
                                    }));
                                }
                                else
                                {
                                    log.Info($"Dar de baja CTG, Dto: {item.Dto.NroCartaPorte}, workflowId: {item.WorkflowId}");
                                    resultados.Add(servicioComandos.Ejecutar(new DarDeBajaCTG
                                    {
                                        Dto = item.Dto,
                                        Vehiculo = item.Vehiculo,
                                        CentroId = item.CentroId,
                                        WorkflowId = item.WorkflowId
                                    }));
                                }
                            }
                            catch (Exception e)
                            {
                                log.Error("No se pudo realizar la baja de CTG: {0}", e.Message);
                            }
                        }
                        else if (item.EstadoCtg == EstadoTransmisionASap.Correcto && item.EstadoCtgDefinitivo == EstadoTransmisionASap.Error)
                        {
                            try
                            {
                                if (item.Dto.Cpe)
                                {
                                    log.Info($"Confirmacion Definitiva, CTG: {item.Dto.CTG}, workflowId: {item.WorkflowId}");
                                    resultados.Add(servicioComandos.Ejecutar(new ConfirmarArriboDefinitivo
                                    {
                                        CentroId = item.CentroId,
                                        Dto = item.Dto,
                                        WorkflowId = item.WorkflowId
                                    }));
                                }
                                else
                                {
                                    log.Info($"Dar de baja CTG Definitiva, Dto: {item.Dto.NroCartaPorte}, workflowId: {item.WorkflowId}");
                                    resultados.Add(servicioComandos.Ejecutar(new DarDeBajaCTGDefinitivo
                                    {
                                        CentroId = item.CentroId,
                                        Dto = item.Dto,
                                        WorkflowId = item.WorkflowId
                                    }));
                                }
                            }
                            catch (Exception e)
                            {
                                log.Error("No se pudo realizar la baja de CTG Definitiva: {0}", e.Message);
                            }
                        }
                    }
                    string content;
                    if (resultados.All(x => !x.HayErrores))
                    {
                        content = "OK";
                    }
                    else if (resultados.Any(x => !x.HayErrores))
                    {
                        content = "W-" + Textos.PanelDeControlTransSAP__MensajeAdvertencia;
                    }
                    else
                    {
                        content = "E-" + Textos.PanelDeControlTransSAP__MensajeError;
                    }

                    log.Info("Transmision terminada: {0}", content);
                }
                else
                {
                    log.Info($"No se obtuvieron registros, cantidad de bajas: {bajaCTGconError.Count()}");
                }
            }
            catch (Exception e)
            {
                log.Error("Ocurrio un problema al realizar el proceso: {}", e.Message);
            }
        }

        public void CachearCpeAfip(int centro = 5, int reintentos = 3, string tipoCpe = "AMBOS")
        {
            var cpesPendientes = (ResultadoConsultaCpePorDestino)servicioComandos.Ejecutar(
                new ConsultarCPEPorDestino()
                {
                    CentroId = centro,
                    TipoCpe = tipoCpe == "CAMION" ? TipoCpeConsulta.Camion : (tipoCpe == "TREN" ? TipoCpeConsulta.Tren : TipoCpeConsulta.Ambos),
                    FechaPartidaDesde = DateTime.Today,
                    FechaPartidaHasta = DateTime.Today.AddDays(1)
                });

            if (!cpesPendientes.HayErrores)
            {
                servicioRepositorio.ActualizarFechaEstadoCacheadoCPECentro(centro, null);
                var cpesNoCacheadas = servicioRepositorio.ObtenerCpesNoCacheadas(cpesPendientes.Cpes);
                var consultasParaleloVal = servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.AFIP, Constantes.ConfiguracionGeneral.AFIP.ConsultasParalelas);

                var consultasParalelo = 0;
                int.TryParse(consultasParaleloVal?.Valor, out consultasParalelo);

                log.Debug("Se inicia el proceso de cacheo");
                Parallel.ForEach(cpesNoCacheadas, new ParallelOptions { MaxDegreeOfParallelism = consultasParalelo }, (ctg) =>
                 {
                     var intentos = 0;
                     var ok = false;

                     while (!ok && intentos < reintentos)
                     {
                         try
                         {
                             var cpe = cpesPendientes.Cpes.First(x => x.Ctg == ctg);

                             var resultado = servicioComandos.Ejecutar(new ConsultarCPDigital()
                             {
                                 CentroId = centro,
                                 TipoVehiculo = cpe.TipoCartaPorte == 79 ? (int)TipoVehiculo.Tren : (int)TipoVehiculo.Camión,
                                 NroCtg = ctg,
                                 ConsultaAfip = true,
                                 FechaUltimaActualizacion = cpe.FechaUltimaModificacion
                             });

                             ok = !resultado.HayErrores;
                         }
                         catch (Exception ex)
                         {
                             log.Error("Ocurrio un problema al realizar el cache de {0}: {1}", ctg, ex.Message);
                             ok = false;
                         }
                         finally
                         {
                             intentos++;
                         }
                     }
                 });
            }
            else
            {
                servicioRepositorio.ActualizarFechaEstadoCacheadoCPECentro(centro, cpesPendientes.Errores.FirstOrDefault().Value);
                log.Error("Ocurrio un problema al realizar ConsultarCPEPorDestino: {0}", cpesPendientes.Errores.FirstOrDefault().Value);
            }
        }

        public void SinscronizarCartaPorteElectronicaActivos(int centro = 5, int items = 10000)
        {
            try
            {
                var paginacion = new Paginacion("CTG", default, 1, itemsPorPagina: items);
                var filtro = new MonitorCPECacheadaFiltroDto
                {
                    CentroId = centro,
                    EsJobAutomatico = true
                };

                var registros = servicioRepositorio.ListarCPEsCacheadas(filtro, paginacion);

                foreach (var cpe in registros.MonitorCPECacheadaListado)
                {
                    var resultado = servicioComandos.Ejecutar(new ConsultarCPDigital()
                    {
                        CentroId = centro,
                        TipoVehiculo = cpe.TipoCartaPorte == 79 ? (int)TipoVehiculo.Tren : (int)TipoVehiculo.Camión,
                        NroCtg = cpe.CTG.Value,
                        ConsultaAfip = true,
                        FechaUltimaActualizacion = cpe.FechaUltimaActualizacion
                    });

                    if (resultado.HayErrores)
                    {
                        foreach (var error in resultado.Errores)
                        {
                            log.Error("Ocurrio un problema al realizar el proceso de SinscronizarEstadosCartaPorteElectronicaActivos del CTG: {0} - {1}", cpe.CTG.Value, error);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Ocurrio un problema al realizar el proceso de SinscronizarEstadosCartaPorteElectronicaActivos: ", e.Message);
            }
        }
    }
}