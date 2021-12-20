using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{

    [Autorizacion(PermisosScato.GeneracionMicromuestras)]
    public class MicromuestraController : BaseController
{
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public MicromuestraController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, MicromuestraModel model)
        {
            var muestra = servicio.ObtenerMuestraEnvioACamaraPorNumero(datosUsuario.CentroId, model.CodigoBarras);
            var impresora = servicio.ObtenerImpresora(model.ImpresoraId);
            var casilleros = servicio.ListarCasillerosPorCentro(datosUsuario.CentroId).OrderBy(x => x.Numero).ToList();

            var dto = new ImpIdentificacionMicromuestraDto();
            var nrosCasilleros = new List<String>();
            var listMicroMuestrasPorCasilleroDto = new List<MicroMuestrasPorCasilleroDto>();
            var microMuestras = servicio.ListarMicroMuestrasPorCasilleroPorCentro(datosUsuario.CentroId).ToList();
            bool hayCasilleroLibre = false;
            int cont = 0;

            if (muestra == null)
            {
                ModelState.AddModelError("CodigoBarras", Textos.Lote_MuestraInexistente);
                ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
                return View();
            }
            if (impresora == null)
            {
                ModelState.AddModelError("ImpresoraId", Textos.Error_Requerido);
                ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
                return View();
            }
            if (muestra.CentroId != model.CentroId)
            {
                ModelState.AddModelError("Centro", Textos.Micromuestra_MuestraError);
                ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
                return View();
            }

            decimal? valorHumedad = null;
            var caracteristicaAnalizada = servicio.ListarAnalisisYCaladoPorId(muestra.CaladoId).LastOrDefault(f => f.EsHumedad);
            if (caracteristicaAnalizada != null)
            {
                valorHumedad = caracteristicaAnalizada.ValorAnalisis ?? caracteristicaAnalizada.ValorCalado;
            }
            var resultado = new Resultado();

            try
            {
                //Asignación de Casillero
                if (casilleros.Count == 0)
                {
                    TempData["Alerta"] = Textos.MicroMuestra_NoExisteCasillero;
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
                    return View();
                }

                for (int i = 0; i < 2; i++)
                {
                    hayCasilleroLibre = false;

                    while (!hayCasilleroLibre && !resultado.HayErrores && cont < casilleros.Count())
                    {
                        if (microMuestras.Count(x => x.CasilleroId == casilleros[cont].Id) < casilleros[cont].Capacidad)
                        {
                            hayCasilleroLibre = true;

                            nrosCasilleros.Add(casilleros[cont].Numero);

                            var dtoMicroMuestra = new MicroMuestrasPorCasilleroDto
                            {
                                MuestraId = muestra.Id,
                                CasilleroId = casilleros[cont].Id,
                                Fecha = DateTime.Now
                            };

                            listMicroMuestrasPorCasilleroDto.Add(dtoMicroMuestra);
                            microMuestras.Add(dtoMicroMuestra);
                            cont--;
                        }
                        cont++;
                    }
                }

                if (!hayCasilleroLibre && !resultado.HayErrores)
                {
                    TempData["Alerta"] = Textos.MicroMuestra_NoExisteCasilleroLibre;
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
                    return View();
                }

                foreach (var microMuestra in listMicroMuestrasPorCasilleroDto)
                {
                    resultado = servicioComandos.Ejecutar(new CrearMicroMuestrasPorCasillero { Dto = microMuestra });
                }

                if (!resultado.HayErrores)
                {
                    muestra.GeneroMicroMuestras = true;
                    resultado = servicioComandos.Ejecutar(new ModificarEnvioACamara { Dto = muestra });
                }

                if(!resultado.HayErrores)
                {
                    dto = new ImpIdentificacionMicromuestraDto
                    {
                        Centro = model.Centro,
                        Impresora = impresora.Direccion,
                        Patente = muestra.Patente,
                        Humedad = valorHumedad != null ? valorHumedad.Value.Formatted() + " %" : string.Empty,
                        Material = muestra.Material,
                        NroMuestra = muestra.NroMuestra,
                        Proveedor = muestra.Proveedor,
                        WorkflowId = muestra.WorkflowInstanceId,
                        NumeroDeOrden = servicio.ObtenerRecorridoIdPorGuid(muestra.WorkflowInstanceId).ToString(CultureInfo.InvariantCulture),
                        Codigo = "IdentificacionMicromuestra",
                        FechaImpresion = DateTime.Now
                    };
                }

                if (muestra.TieneAnalisisInterno && !resultado.HayErrores)
                {
                    dto.NroCasillero = null;
                    resultado = servicioComandos.Ejecutar(new ImprimirMicromuestra { Dto = dto, TipoMicromuestra = TipoMicromuestra.AnalisisInterno });
                }
                if (!resultado.HayErrores)
                {
                    dto.NroCasillero = null;
                    resultado = servicioComandos.Ejecutar(new ImprimirMicromuestra { Dto = dto, TipoMicromuestra = TipoMicromuestra.Camara });
                }
                if (!resultado.HayErrores)
                {
                    dto.NroCasillero = nrosCasilleros[0];
                    resultado = servicioComandos.Ejecutar(new ImprimirMicromuestra { Dto = dto, TipoMicromuestra = TipoMicromuestra.Casillero });
                }
                if (!resultado.HayErrores)
                {
                    dto.NroCasillero = nrosCasilleros[1];
                    resultado = servicioComandos.Ejecutar(new ImprimirMicromuestra { Dto = dto, TipoMicromuestra = TipoMicromuestra.Casillero });
                }
                if (!resultado.HayErrores)
                {
                    dto.NroCasillero = null;
                    resultado = servicioComandos.Ejecutar(new ImprimirMicromuestra { Dto = dto, TipoMicromuestra = TipoMicromuestra.Entregador });
                }
            }
            catch (Exception e)
            {
                TempData["Alerta"] = Textos.Micromuestra_Error;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                resultado.Errores.Add("", e.Message);
                ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
                return View(model);
            }

            if (resultado.HayErrores)
            {
                TempData["Alerta"] = resultado.Errores.First().Value;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
                return View(model); 
            }

            TempData["Alerta"] = Textos.Micromuestra_Ok;
            TempData["TipoAlerta"] = TipoAlerta.Exito;
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            return View(model);
        }

    }
}
