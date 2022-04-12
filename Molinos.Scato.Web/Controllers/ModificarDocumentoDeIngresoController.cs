using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(new[] { PermisosScato.AbmModificarDocumentoDeIngreso, PermisosScato.AbmConsultarDocumentoDeIngreso })]
    public class ModificarDocumentoDeIngresoController : DocumentoIngresoController
    {
        private IListaDeWorkflows ListaDeWorkflows;

        public ModificarDocumentoDeIngresoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows listaDeWorkflows)
            : base(log, servicio, servicioComandos)
        {
            ListaDeWorkflows = listaDeWorkflows;
        }

        [DatosUsuario]
        public ActionResult Index(string error, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Patente", DirOrden dirOrden = DirOrden.Asc, bool soloLectura = false, bool cargarCookie = true, bool filtrarPorTarjeta = false)
        {
            ViewBag.SoloLectura = soloLectura;
            ViewBag.FiltrarPorTarjeta = filtrarPorTarjeta;
            var model = new ModificarDocumentoDeIngresoDto
            {
                TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                NumeroDocumentoIngreso = "0",
                NumeroDeTarjeta = "0",
                Patente = "0"
            };
            if (cargarCookie)
            {
                model = ObtenerDatosCookie(model);
            }

            ListarConsulta(datosUsuario.CentroId, pagina, ordenarPor, dirOrden, model);

            if (!String.IsNullOrEmpty(error))
            {
                ModelState.AddModelError("NumeroDocumentoIngreso", error);
            }
            if (cargarCookie)
            {
                return View(model);
            }
            return View();
        }

        private ModificarDocumentoDeIngresoDto ObtenerDatosCookie(ModificarDocumentoDeIngresoDto filtro)
        {
            var cookie = new CookieUsuario();

            filtro.Patente = cookie.Valor("DocumentoIngresoPatente");
            filtro.NumeroDocumentoIngreso = cookie.Valor("DocumentoIngresoNumeroDocumentoDeIngreso");
            if (cookie.Valor("DocumentoIngresoFechaTaraDesde") != "" && cookie.Valor("DocumentoIngresoFechaTaraDesde") != null)
            {
                filtro.FechaTaraDesdeCP = DateTime.Parse(cookie.Valor("DocumentoIngresoFechaTaraDesde"));
            }
            if (cookie.Valor("DocumentoIngresoFechaTaraHasta") != "" && cookie.Valor("DocumentoIngresoFechaTaraHasta") != null)
            {
                filtro.FechaTaraHastaCP = DateTime.Parse(cookie.Valor("DocumentoIngresoFechaTaraHasta"));
            }
            if (cookie.Valor("DocumentoIngresoFechaDesdeCP") != "" && cookie.Valor("DocumentoIngresoFechaDesdeCP") != null)
            {
                filtro.FechaDesdeCP = DateTime.Parse(cookie.Valor("DocumentoIngresoFechaDesdeCP"));
            }
            if (cookie.Valor("DocumentoIngresoFechaHastaCP") != "" && cookie.Valor("DocumentoIngresoFechaHastaCP") != null)
            {
                filtro.FechaHastaCP = DateTime.Parse(cookie.Valor("DocumentoIngresoFechaHastaCP"));
            }

            filtro.Corredor = cookie.Valor("DocumentoIngresoRemitenteComercial");
            filtro.TitularCartaPorte = cookie.Valor("DocumentoIngresoTitularCP");
            filtro.MaterialDesc = cookie.Valor("DocumentoIngresoMaterialDesc");

            if (int.TryParse(cookie.Valor("DocumentoIngresoRemitenteComercialId"), out int rtte))
            {
                filtro.CorredorId = rtte;
            }
            if (int.TryParse(cookie.Valor("DocumentoIngresoTitularCPId"), out int titular))
            {
                filtro.TitularCartaPorteId = titular;
            }
            if (int.TryParse(cookie.Valor("DocumentoIngresoMaterialDescId"), out int material))
            {
                filtro.MaterialId = material;
            }
            var valor = cookie.Valor("TipoDocumentoDeIngresoCookie");
            TipoDocumentoIngreso tipoDocumentoDeIngresoCookie;
            if (!string.IsNullOrEmpty(valor) && Enum.TryParse<TipoDocumentoIngreso>(valor, out tipoDocumentoDeIngresoCookie))
            {
                filtro.TipoDocumentoIngreso = tipoDocumentoDeIngresoCookie;
            }
            return filtro;
        }

        private ModificarDocumentoDeIngresoDto ActualizarCookie(ModificarDocumentoDeIngresoDto filtro)
        {
            var cookie = new CookieUsuario();

            cookie.ActualizarValor("DocumentoIngresoPatente", filtro.Patente);
            cookie.ActualizarValor("TipoDocumentoDeIngresoCookie", filtro.TipoDocumentoIngreso.ToString());
            cookie.ActualizarValor("DocumentoIngresoNumeroDocumentoDeIngreso", filtro.NumeroDocumentoIngreso);
            cookie.ActualizarValor("DocumentoIngresoFechaTaraDesde", filtro.FechaTaraDesdeCP.ToString());
            cookie.ActualizarValor("DocumentoIngresoFechaTaraHasta", filtro.FechaTaraHastaCP.ToString());
            cookie.ActualizarValor("DocumentoIngresoFechaDesdeCP", filtro.FechaDesdeCP.ToString());
            cookie.ActualizarValor("DocumentoIngresoFechaHastaCP", filtro.FechaHastaCP.ToString());

            cookie.ActualizarValor("DocumentoIngresoRemitenteComercial", filtro.Corredor);
            cookie.ActualizarValor("DocumentoIngresoTitularCP", filtro.TitularCartaPorte);
            cookie.ActualizarValor("DocumentoIngresoMaterialDesc", filtro.MaterialDesc);

            cookie.ActualizarValor("DocumentoIngresoRemitenteComercialId", filtro.CorredorId.ToString());
            cookie.ActualizarValor("DocumentoIngresoTitularCPId", filtro.TitularCartaPorteId.ToString());
            cookie.ActualizarValor("DocumentoIngresoMaterialDescId", filtro.MaterialId.ToString());
            return filtro;
        }

        public ActionResult DocumentoOrigen(int recorridoId, TipoDocumentoIngreso tipoDoc, bool soloLectura, bool esDocumentoDeOrigen = false, bool filtrarPorTarjeta = false, bool actualizarCookie = false, bool tieneDatosExportacion = false)
        {
            ViewBag.SoloLectura = soloLectura;
            ViewBag.RecorridoId = recorridoId;
            ViewBag.EsDocumentoDeOrigen = esDocumentoDeOrigen;
            ViewBag.FiltrarPorTarjeta = filtrarPorTarjeta;
            ViewBag.TipoDocumentoString = EspacioEntreMayusculas(tipoDoc.ToString());
            ViewBag.TipoDocumento = tipoDoc;
            ViewBag.TieneDatosExportacion = tieneDatosExportacion;

            var recorrido = servicio.ObtenerRecorrido(recorridoId);

            if (recorrido != null)
            {
                ViewBag.Rechazado = recorrido.Rechazado;
                ViewBag.Patente = recorrido.Patente;
                ViewBag.NumeroDocumento = recorrido.NumeroDocumentoIngreso;
            }

            if (actualizarCookie)
            {
                ActualizarCookie(new ModificarDocumentoDeIngresoDto
                {
                    TipoDocumentoIngreso = tipoDoc,
                    NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                    NumeroDeTarjeta = "",
                    Patente = recorrido.Patente
                });
            }

            object documentoDeIngreso;
            if (tipoDoc == TipoDocumentoIngreso.CartaPorte)
            {
                var documentoDeIngresoAux = servicio.ObtenerCartaPorte(recorrido.Vehiculo.CartaPorteId);
                var centro = servicio.ObtenerCentro(recorrido.Centro.Id);
                documentoDeIngresoAux.TipoDeWorkflow = recorrido.Workflow.TipoDeWorkflow;
                documentoDeIngresoAux.LeerCPDeFoto = centro.LeerCPDeFoto;
                documentoDeIngreso = documentoDeIngresoAux;
                var foto = servicio.ObtenerFotoCPDeCartaDePortePorrecorrido(recorrido.InstanciaWorkflow);
                if (foto.Fotos.Any())
                {
                    ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
                }
                if (documentoDeIngresoAux.TipoVehiculo == TipoVehiculo.Tren)
                {
                    foreach (var vehiculo in documentoDeIngresoAux.Vehiculos)
                    {
                        vehiculo.NumOrden = documentoDeIngresoAux?.CTG;
                        vehiculo.Sucural = documentoDeIngresoAux?.Sucursal?.ToString("D5");
                        vehiculo.NumCTG = documentoDeIngresoAux?.NroCartaPorte;
                    }
                }
                CargarCartaPorteController.SetearVista(recorrido.Workflow, recorrido.Centro.Id, servicio, this);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.OrdenCargaInterna))
            {
                documentoDeIngreso = servicio.ObtenerOrdenCargaInternaPorInstanceId(recorrido.InstanciaWorkflow);
                IngresarOrdenCargaInternaController.SetearVista(recorrido.Workflow, recorrido.Centro.Id, servicio, this);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.OrdenDeDescarga))
            {
                documentoDeIngreso = servicio.ObtenerOrdenDeDescargaPorNumero(recorrido.NumeroDocumentoIngreso);
                CargarOrdenDeDescargaController.SetearVista(recorrido.Workflow, servicio, this);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.OrdenCargaFas))
            {
                var orden = servicio.ObtenerOrdenCargaFasPorInstanceId(recorrido.InstanciaWorkflow);
                IngresarOrdenCargaFasController.SetearVista(recorrido.Workflow, servicio, this);

                return View("OrdenCargaFas", orden);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.OrdenCargaInternaFason))
            {
                documentoDeIngreso = servicio.ObtenerOrdenCargaInternaFasonPorInstanceId(recorrido.InstanciaWorkflow);
                IngresarOrdenCargaInternaFasonController.SetearVista(recorrido.Workflow, recorrido.Centro.Id, servicio, this);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.OrdenDeDescargaFason))
            {
                documentoDeIngreso = servicio.ObtenerOrdenDeDescargaFasonPorNumero(recorrido.NumeroDocumentoIngreso);
                CargarOrdenDeDescargaFasonController.SetearVista(recorrido.Workflow, servicio, this, recorrido.Centro.Id);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.OrdenEntrePlantas))
            {
                documentoDeIngreso = servicio.ObtenerOrdenEntrePlantasPorInstanceId(recorrido.InstanciaWorkflow);
                CargarOrdenEntrePlantasController.SetearVista(recorrido.Workflow, servicio, this);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.OrdenDeCargaContenedor))
            {
                documentoDeIngreso = servicio.ObtenerOrdenDeCargaContenedorPorInstanceId(recorrido.InstanciaWorkflow);
                CargarOrdenDeCargaContenedorController.SetearVista(recorrido.Workflow, servicio, this);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.Remito))
            {
                documentoDeIngreso = servicio.ObtenerRemitoPorOrdenDeDescarga(recorrido.NumeroDocumentoIngreso);
                //(documentoDeIngreso as RemitoDto).Almacen_Id = servicio.ObtenerAlmacenPorRecorrido((documentoDeIngreso as RemitoDto).RecorridoId);
                IngresoRemitoController.SetearVista(recorrido.Workflow, servicio, this);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.RemitoBodegaUvaPropia) && soloLectura)
            {
                documentoDeIngreso = servicio.ObtenerRemitoBodegaUvaPorInstanceId(recorrido.InstanciaWorkflow);
                RemitoBodegaUvaPropiaController.SetearVista(recorrido.Workflow, recorrido.Centro.Id, servicio, this);
                ViewBag.Materiales = servicio.ListarMaterialesPorWorkflowYVinedo(recorrido.Workflow.Id, recorrido.Workflow.CentroId, ((RemitoBodegaUvaDto)documentoDeIngreso).VinedoId).ToSelectList(f => f.MaterialId.ToString(CultureInfo.InvariantCulture), f => f.MaterialDesc);
                ViewBag.TiposDeBines = new List<SelectListItem>();
            }
            else if ((tipoDoc == TipoDocumentoIngreso.RemitoBodegaUvaTerceros) && soloLectura)
            {
                documentoDeIngreso = servicio.ObtenerRemitoBodegaUvaPorInstanceId(recorrido.InstanciaWorkflow);
                RemitoBodegaUvaTercerosController.SetearVista(recorrido.Workflow, recorrido.Centro.Id, servicio, this);
                ViewBag.Materiales = servicio.ListarMaterialesPorWorkflowYVinedo(recorrido.Workflow.Id, recorrido.Workflow.CentroId, ((RemitoBodegaUvaDto)documentoDeIngreso).VinedoId).ToSelectList(f => f.MaterialId.ToString(CultureInfo.InvariantCulture), f => f.MaterialDesc);
                ViewBag.TiposDeBines = new List<SelectListItem>();
            }
            else if ((tipoDoc == TipoDocumentoIngreso.RemitoBodegaVino) && soloLectura)
            {
                documentoDeIngreso = servicio.ObtenerRemitoBodegaVinoPorInstanceId(recorrido.InstanciaWorkflow);
                RemitoBodegaVinoController.SetearVista(recorrido.Workflow, recorrido.Centro.Id, servicio, this);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.HojaDeRuta) && soloLectura)
            {
                documentoDeIngreso = servicio.ObtenerHojaDeRutaPorInstanceId(recorrido.InstanciaWorkflow);
                CargarHojaDeRutaController.SetearVista(recorrido.Workflow, servicio, this);
            }
            else if ((tipoDoc == TipoDocumentoIngreso.HojaDeRutaYerbatera) && soloLectura)
            {
                documentoDeIngreso = servicio.ObtenerHojaDeRutaYerbateraPorInstanceId(recorrido.InstanciaWorkflow);
                CargarHojaDeRutaYerbateraController.SetearVista(recorrido.Workflow, recorrido.Centro.Id, servicio, this);
            }
            else
            {
                return RedirectToAction("Index", new { error = Textos.Error_DocumentoIngresadoNoModificable, soloLectura, filtrarPorTarjeta });
            }
            return View(tipoDoc.ToString(), documentoDeIngreso);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult OrdenCargaInterna(string workflow, OrdenCargaInternaDto orden, int centroId, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;
            orden.Almacen_Id = servicio.ObtenerAlmacenPorRecorrido(orden.RecorridoId);

            if (orden.PatenteCamion != null)
            {
                orden.PatenteCamion = orden.PatenteCamion.ToUpper();
            }
            if (orden.PatenteAcoplado != null)
            {
                orden.PatenteAcoplado = orden.PatenteAcoplado.ToUpper();
            }
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (!resultadoChofer)
            {
                IngresarOrdenCargaInternaController.SetearVista(workflowObj, centroId, servicio, this);
                return View(orden);
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                IngresarOrdenCargaInternaController.SetearVista(workflowObj, centroId, servicio, this);
                return View(orden);
            }

            var resultado = servicioComandos.Ejecutar(new ModificarOrdenCargaInterna { Orden = orden, NombreWorkflow = workflow, NombreUsuario = datosUsuario.NombreUsuario });

            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            ModelState.AgregarErrores(resultado);
            IngresarOrdenCargaInternaController.SetearVista(workflowObj, centroId, servicio, this);
            return View(orden);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult CartaPorte(string workflow, CartaPorteDto orden, int centroId, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = 0;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var vehiculos = orden.Vehiculos;

            if (ModelState.IsValid && vehiculos != null && vehiculos.Count() != 0)
            {
                var resultadoChofer = SetearChofer(orden.Chofer);
                if (!resultadoChofer)
                {
                    CargarCartaPorteController.SetearVista(workflowObj, centroId, servicio, this);
                    return View(orden);
                }

                var transportistaId = orden.TransportistaId ?? 0;
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    CargarCartaPorteController.SetearVista(workflowObj, centroId, servicio, this);
                    return View(orden);
                }

                orden.Vehiculos = vehiculos;
                var i = 1;
                foreach (var vehiculo in vehiculos)
                {
                    vehiculo.TipoVehiculo = orden.TipoVehiculo;
                    vehiculo.Patente = vehiculo.Patente != null ? vehiculo.Patente.ToUpper() : "";
                    vehiculo.PatenteAcoplado = vehiculo.PatenteAcoplado != null ? vehiculo.PatenteAcoplado.ToUpper() : "";
                    vehiculo.NumeroVehiculo = i++;
                }

                var resultado = servicioComandos.Ejecutar(new ModificarCartaPorte { Orden = orden, NombreUsuario = datosUsuario.NombreUsuario });

                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            if (vehiculos == null || !vehiculos.Any())
            {
                ModelState.AddModelError("Vehiculo", string.Format(Textos.Error_Requerido, "Vagones"));
            }

            CargarCartaPorteController.SetearVista(workflowObj, centroId, servicio, this);
            return View(orden);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult OrdenDeDescargaFason(string workflow, OrdenDeDescargaFasonDto orden, int centroId, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (!resultadoChofer)
            {
                CargarOrdenDeDescargaFasonController.SetearVista(workflowObj, servicio, this, centroId);
                return View(orden);
            }

            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                CargarOrdenDeDescargaFasonController.SetearVista(workflowObj, servicio, this, centroId);
                return View(orden);
            }

            orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;

            orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
            orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";

            var resultado = servicioComandos.Ejecutar(new ModificarOrdenDeDescargaFason { Orden = orden });

            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            CargarOrdenDeDescargaFasonController.SetearVista(workflowObj, servicio, this, centroId);
            return View(orden);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult OrdenDeDescarga(string workflow, OrdenDeDescargaDto orden, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (!resultadoChofer)
            {
                CargarOrdenDeDescargaController.SetearVista(workflowObj, servicio, this);
                return View(orden);
            }

            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                CargarOrdenDeDescargaController.SetearVista(workflowObj, servicio, this);
                return View(orden);
            }

            orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;

            orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
            orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";

            var resultado = servicioComandos.Ejecutar(new ModificarOrdenDeDescarga { Orden = orden, NombreUsuario = datosUsuario.NombreUsuario });

            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            CargarOrdenDeDescargaController.SetearVista(workflowObj, servicio, this);
            return View(orden);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult OrdenEntrePlantas(string workflow, OrdenEntrePlantasDto orden, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var resultadoChofer = SetearChofer(orden.Chofer);
            if (!resultadoChofer)
            {
                CargarOrdenEntrePlantasController.SetearVista(workflowObj, servicio, this);
                return View(orden);
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                CargarOrdenEntrePlantasController.SetearVista(workflowObj, servicio, this);
                return View(orden);
            }
            if (ModelState.IsValid)
            {
                orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
                orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";
                var resultado = servicioComandos.Ejecutar(new ModificarOrdenEntrePlantas { Orden = orden, NombreUsuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarOrdenEntrePlantasController.SetearVista(workflowObj, servicio, this);
            return View(orden);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult OrdenCargaFas(string workflow, OrdenCargaFasDto orden, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;

            ViewBag.RecorridoId = orden.RecorridoId;
            var workflowObjt = servicio.ObtenerWorkflowPorCodigo(workflow);
            if (ModelState.IsValid)
            {
                if (orden.PatenteCamion != null)
                {
                    orden.PatenteCamion = orden.PatenteCamion.ToUpper();
                }
                if (orden.PatenteAcoplado != null)
                {
                    orden.PatenteAcoplado = orden.PatenteAcoplado.ToUpper();
                }

                var resultadoChofer = SetearChofer(orden.Chofer);
                if (!resultadoChofer)
                {
                    IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                    return View(orden);
                }
                var transportista = servicio.ObtenerTransportistaPorCuit(orden.CuitTransporte);
                if (transportista != null) //Transportista Existente
                {
                    orden.TransportistaId = transportista.Id;
                }
                else //Creo el nuevo transportista
                {
                    var proveedor = servicio.ObtenerProveedorPorCuit(orden.CuitTransporte, new TiposProveedor { PR = true });
                    var resultadoTransportista = servicioComandos.Ejecutar(new CrearTransportista
                    {
                        Dto = new TransportistaDto
                        {
                            Cuit = proveedor.Cuil,
                            Domicilio = proveedor.Domicilio,
                            LocalidadId = proveedor.LocalidadId,
                            ProvinciaId = proveedor.ProvinciaId,
                            RazonSocial = proveedor.RazonSocial
                        }
                    });
                    if (resultadoTransportista.HayErrores)
                    {
                        resultadoTransportista.Errores.ToList()
                                                .ForEach(f => ModelState.AddModelError("Transportista", f.Value));
                        ModelState.AgregarErrores(resultadoTransportista);
                        IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                        return View(orden);
                    }
                    orden.TransportistaId = ((ResultadoCrear)resultadoTransportista).Id;
                }

                var resultado = servicioComandos.Ejecutar(new ModificarOrdenCargaFas { Orden = orden, NombreUsuario = datosUsuario.NombreUsuario });

                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
                IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                return View(orden);
            }
            IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
            return View(orden);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult OrdenCargaInternaFason(string workflow, OrdenCargaInternaFasonDto orden, int centroId, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;

            if (orden.PatenteCamion != null)
            {
                orden.PatenteCamion = orden.PatenteCamion.ToUpper();
            }
            if (orden.PatenteAcoplado != null)
            {
                orden.PatenteAcoplado = orden.PatenteAcoplado.ToUpper();
            }

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (!resultadoChofer)
            {
                IngresarOrdenCargaInternaFasonController.SetearVista(workflowObj, centroId, servicio, this);
                return View(orden);
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                IngresarOrdenCargaInternaFasonController.SetearVista(workflowObj, centroId, servicio, this);
                return View(orden);
            }
            var resultado = servicioComandos.Ejecutar(new ModificarOrdenCargaInternaFason { Orden = orden, NombreWorkflow = workflow, NombreUsuario = datosUsuario.NombreUsuario });
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            IngresarOrdenCargaInternaFasonController.SetearVista(workflowObj, centroId, servicio, this);
            return View(orden);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult OrdenDeCargaContenedor(string workflow, OrdenDeCargaContenedorDto orden, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;

            orden.PesoContenedorEntrada = servicio.ObtenerTaraContenedor(orden.ContenedorEntradaId).PesoTara;
            orden.PesoContenedorSalida = servicio.ObtenerTaraContenedor(orden.ContenedorSalidaId).PesoTara;

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (!resultadoChofer)
            {
                CargarOrdenDeCargaContenedorController.SetearVista(workflowObj, servicio, this);
                return View(orden);
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                CargarOrdenDeCargaContenedorController.SetearVista(workflowObj, servicio, this);
                return View(orden);
            }

            orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
            orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";

            var resultado = servicioComandos.Ejecutar(new ModificarOrdenDeCargaContenedor { Orden = orden, NombreWorkflow = workflow, NombreUsuario = datosUsuario.NombreUsuario });
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            CargarOrdenDeCargaContenedorController.SetearVista(workflowObj, servicio, this);
            return View(orden);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult Remito(string workflow, RemitoDto orden, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;
            orden.Almacen_Id = servicio.ObtenerAlmacenPorRecorrido(orden.RecorridoId);

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var resultadoChofer = SetearChofer(orden.Chofer);
            if (!resultadoChofer)
            {
                IngresoRemitoController.SetearVista(workflowObj, servicio, this);
                return View(orden);
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                IngresoRemitoController.SetearVista(workflowObj, servicio, this);
                return View(orden);
            }
            orden.OrigenCodigoSap = orden.EsRemitoProveedor ? servicio.ObtenerProveedor(orden.OrigenId).CodigoSap : servicio.ObtenerCentro(orden.OrigenId).CodigoSAP;
            orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;
            orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
            orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";
            orden.Remito = orden.Remito.Replace('-', 'R');
            var resultado = servicioComandos.Ejecutar(new ModificarRemito { Orden = orden, NombreWorkflow = workflow, NombreUsuario = datosUsuario.NombreUsuario });
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            IngresoRemitoController.SetearVista(workflowObj, servicio, this);
            return View(orden);
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmModificarDocumentoDeIngreso)]
        public ActionResult RemitoBodegaVino(string workflow, RemitoBodegaVinoDto orden, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var resultadoChofer = SetearChofer(orden.Chofer);
            if (!resultadoChofer)
            {
                RemitoBodegaVinoController.SetearVista(workflowObj, datosUsuario.CentroId, servicio, this);
                return View(orden);
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                RemitoBodegaVinoController.SetearVista(workflowObj, datosUsuario.CentroId, servicio, this);
                return View(orden);
            }
            orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;
            orden.Patente = orden.Patente != null ? orden.Patente.ToUpper() : "";
            var resultado = servicioComandos.Ejecutar(new ModificarRemitoBodegaVino { Orden = orden, NombreWorkflow = workflow, NombreUsuario = datosUsuario.NombreUsuario });
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            RemitoBodegaVinoController.SetearVista(workflowObj, datosUsuario.CentroId, servicio, this);
            return View(orden);
        }

        public ActionResult Pesada(int recorridoId, TipoDocumentoIngreso tipoDoc, string numeroDoc, string patente, bool filtrarPorTarjeta, bool tieneDatosExportacion = false)
        {
            var recorrido = servicio.ObtenerRecorrido(recorridoId);
            ViewBag.FiltrarPorTarjeta = filtrarPorTarjeta;
            if (recorrido != null)
            {
                var actividad = recorrido.Terminado ? Textos.Terminado : "";
                actividad = recorrido.Rechazado && recorrido.Terminado ? Textos.Rechazado : actividad;
                var act = actividad == "" ? ListaDeWorkflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow) : new ProximaAccionDto();
                ViewBag.Rechazado = recorrido.Rechazado;
                ViewBag.Actividad = string.IsNullOrEmpty(actividad) && !string.IsNullOrEmpty(act.ProximaAccion) ?
                    Textos.ResourceManager.GetString("Act" + act.ProximaAccion) + (recorrido.Rechazado ? "(" + Textos.Rechazado + ")" : "") :
                    string.IsNullOrEmpty(actividad) && act.Mensaje == Textos.Error_WorkflowNoIdle ? "Procesando Actividad" : actividad;
                ViewBag.FechaCalado = recorrido.AnalisisDeCalidad != null
                                          ? recorrido.AnalisisDeCalidad.FechaCreacion
                                          : recorrido.Calado != null ? recorrido.Calado.FechaCreacion : null;

                var lotes = servicio.ListarLotesPorWorkflow(recorrido.InstanciaWorkflow);
                ViewBag.Lotes = "";
                foreach (var loteDto in lotes)
                {
                    var numero = loteDto.NumeroDeLote;
                    var fecha = loteDto.Fecha.ToShortDateString();
                    var lote = numero + " / " + fecha;
                    if (ViewBag.Lotes == "")
                    {
                        ViewBag.Lotes = ViewBag.Lotes + lote;
                    }
                    else
                    {
                        ViewBag.Lotes = ViewBag.Lotes + " ; " + lote;
                    }
                }

                var docSap = recorrido.NumeroDeDocumentoSap ?? "";
                if (docSap == "")
                {
                    var controles = servicio.ListarControlRecorridoServiciosSap(recorrido.InstanciaWorkflow);
                    foreach (var control in controles)
                    {
                        var mensajeError = control.Comentario;
                        docSap = docSap == "" ? control.Actividad + " / " + mensajeError : docSap + " ; " + control.Actividad + " / " + mensajeError;
                    }
                }

                ViewBag.DocSap = docSap;
                ViewBag.TipoDocumentoString = EspacioEntreMayusculas(tipoDoc.ToString());
                ViewBag.TipoDocumento = tipoDoc;
                ViewBag.NumeroDocumento = recorrido.NumeroDocumentoIngreso;
                ViewBag.BalanzaBruto = recorrido.BalanzaBrutoId.HasValue ? servicio.ObtenerBalanza(recorrido.BalanzaBrutoId.Value).Nombre : "";
                ViewBag.BalanzaTara = recorrido.BalanzaTaraId.HasValue ? servicio.ObtenerBalanza(recorrido.BalanzaTaraId.Value).Nombre : "";
                ViewBag.TieneDatosExportacion = tieneDatosExportacion;

                SetearDatosPestanias(recorridoId, tipoDoc, numeroDoc, patente);
                return View(recorrido);
            }

            return View();
        }

        public ActionResult AnalisisDeCalidad(int recorridoId, TipoDocumentoIngreso tipoDoc, string numeroDoc, string patente, bool filtrarPorTarjeta, bool tieneDatosExportacion = false)
        {
            var recorrido = servicio.ObtenerRecorrido(recorridoId);
            ViewBag.FiltrarPorTarjeta = filtrarPorTarjeta;
            ViewBag.TieneDatosExportacion = tieneDatosExportacion;
            ViewBag.Calador = string.Empty;
            ViewBag.Calle = string.Empty;
            if (recorrido != null)
            {
                ViewBag.Rechazado = recorrido.Rechazado;

                var calidades = new List<CalidadCaracteristicaDto>();
                var muestraEnvioACamara = recorrido.Calado != null ? servicio.ObtenerMuestraEnvioACamaraPorCalado(recorrido.Calado.Id) : null;
                var analisisDeCalidad = recorrido.AnalisisDeCalidad;
            

                if (analisisDeCalidad != null)
                {

                    var puestoDeTrabajo = servicio.ObtenerPuestoDeLogLecturaDeTarjeta(recorrido.Patente, analisisDeCalidad.Id);

                    if (!string.IsNullOrEmpty(puestoDeTrabajo))
                    {
                        var caladorCalle = puestoDeTrabajo.Split('-');
                        if (caladorCalle.Length > 1)
                        {
                            ViewBag.Calador = caladorCalle[0];
                            ViewBag.Calle = caladorCalle[1];
                        }
                        else
                        {
                            ViewBag.Calador = caladorCalle[0];
                            ViewBag.Calle = string.Empty;
                        }
                    }

                    var usuario = servicio.ObtenerUsuarioId(analisisDeCalidad.Usuario);
                    ViewBag.Nombre = usuario != null ? usuario.Nombre : null;
                    ViewBag.Apellido = usuario != null ? usuario.Apellido : null;

                    foreach (var analisisPorCaracteristica in analisisDeCalidad.CaracteristicasAnalizadas)
                    {
                        calidades.Add(new CalidadCaracteristicaDto
                        {
                            Caracteristica = analisisPorCaracteristica.Caracteristica,
                            CaracteristicaId = analisisPorCaracteristica.CaracteristicaId.ToString(CultureInfo.CurrentCulture),
                            DescuentoEnKg = analisisPorCaracteristica.DescuentoEnKg,
                            DescuentoEnPorcentaje = analisisPorCaracteristica.DescuentoEnPorcentaje,
                            MaximoPermitido = analisisPorCaracteristica.Rango.Substring(analisisPorCaracteristica.Rango.IndexOf('-') + 2),
                            Unidad = analisisPorCaracteristica.Unidad,
                            ValorCalado = analisisPorCaracteristica.ValorCalado,
                            ValorAnalisis = analisisPorCaracteristica.ValorAnalisis,
                            SeEnvioACamara = muestraEnvioACamara != null && muestraEnvioACamara.CaracteristicasDeCalidad.Any(x => x.Id == analisisPorCaracteristica.CaracteristicaId),
                            CodigoSap = analisisPorCaracteristica.CaracteristicaCodigoSap
                        });
                    }
                }

                var calado = recorrido.Calado;
                if (calado != null)
                {
                    var huboRecalado = analisisDeCalidad != null && calado.FechaCreacion > analisisDeCalidad.FechaCreacion;

                    if (ViewBag.Nombre == null || huboRecalado)
                    {
                        var calador = servicio.ObtenerUsuarioId(calado.Usuario);
                        ViewBag.Nombre = calador != null ? calador.Nombre : null;
                        ViewBag.Apellido = calador != null ? calador.Apellido : null;
                    }

                    if (huboRecalado)
                    {
                        calidades = calidades.Where(c => !calado.CaladosPorCaracteristica.Any(x => c.CaracteristicaId == x.CaracteristicaId.ToString(CultureInfo.CurrentCulture))).ToList();
                    }

                    //carateristicas no analizadas
                    foreach (var caladoPorCaracteristica in calado.CaladosPorCaracteristica.Where(caladoPorCaracteristica => calidades.FirstOrDefault(x => x.CaracteristicaId == caladoPorCaracteristica.CaracteristicaId.ToString(CultureInfo.CurrentCulture)) == null))
                    {
                        calidades.Add(new CalidadCaracteristicaDto
                        {
                            Caracteristica = caladoPorCaracteristica.Caracteristica,
                            CaracteristicaId = caladoPorCaracteristica.CaracteristicaId.ToString(CultureInfo.CurrentCulture),
                            DescuentoEnKg = caladoPorCaracteristica.DescuentoEnKg,
                            DescuentoEnPorcentaje = caladoPorCaracteristica.DescuentoEnPorcentaje,
                            MaximoPermitido = caladoPorCaracteristica.Rango.Substring(caladoPorCaracteristica.Rango.IndexOf('-') + 2),
                            Unidad = caladoPorCaracteristica.Unidad,
                            ValorCalado = caladoPorCaracteristica.ValorCalado,
                            SeEnvioACamara = muestraEnvioACamara != null && muestraEnvioACamara.CaracteristicasDeCalidad.Any(x => x.Id == caladoPorCaracteristica.CaracteristicaId),
                            CodigoSap = caladoPorCaracteristica.CaracteristicaCodigoSap
                        });
                    }
                    ViewBag.Comentario = calado.Comentario;
                }
                ViewBag.TipoDocumentoString = EspacioEntreMayusculas(tipoDoc.ToString());
                ViewBag.TipoDocumento = tipoDoc;
                ViewBag.NumeroDocumento = recorrido.NumeroDocumentoIngreso;
                SetearDatosPestanias(recorridoId, tipoDoc, numeroDoc, patente);
                ViewBag.Items = new ListaPaginada<CalidadCaracteristicaDto>(calidades, 1, 10, 0);
                return View();
            }

            SetearDatosPestanias(recorridoId, tipoDoc, numeroDoc, patente);
            return View();
        }

        public ActionResult AsignacionDeRuta(int recorridoId, TipoDocumentoIngreso tipoDoc, string numeroDoc, string patente, bool filtrarPorTarjeta, bool tieneDatosExportacion = false)
        {
            var recorrido = servicio.ObtenerRecorrido(recorridoId);
            ViewBag.FiltrarPorTarjeta = filtrarPorTarjeta;
            if (recorrido != null)
            {
                ViewBag.Rechazado = recorrido.Rechazado;
                ViewBag.BalanzaBruto = recorrido.BalanzaBrutoId.HasValue ? servicio.ObtenerBalanza(recorrido.BalanzaBrutoId.Value).Nombre : "";
                ViewBag.BalanzaTara = recorrido.BalanzaTaraId.HasValue ? servicio.ObtenerBalanza(recorrido.BalanzaTaraId.Value).Nombre : "";
                ViewBag.TipoDocumentoString = EspacioEntreMayusculas(tipoDoc.ToString());
                ViewBag.TipoDocumento = tipoDoc;
                ViewBag.NumeroDocumento = recorrido.NumeroDocumentoIngreso;
                ViewBag.Patente = recorrido.Patente;
                ViewBag.TieneDatosExportacion = tieneDatosExportacion;
                SetearDatosPestanias(recorridoId, tipoDoc, numeroDoc, patente);
                return View(recorrido);
            }
            ViewBag.TipoDocumentoString = EspacioEntreMayusculas(tipoDoc.ToString());
            ViewBag.TipoDocumento = tipoDoc;
            ViewBag.NumeroDocumento = numeroDoc;
            ViewBag.Patente = patente;

            SetearDatosPestanias(recorridoId, tipoDoc, numeroDoc, patente);
            return View();
        }

        public ActionResult Fotos(int recorridoId, TipoDocumentoIngreso tipoDoc, string numeroDoc, string patente, bool filtrarPorTarjeta, bool rechazado, bool tieneDatosExportacion = false)
        {
            var fotos = servicio.ListarFotosCamionPorRecorrido(recorridoId, null);
            var recorrido = servicio.ObtenerRecorrido(recorridoId);
            ViewBag.FiltrarPorTarjeta = filtrarPorTarjeta;
            ViewBag.Rechazado = rechazado;
            ViewBag.TipoDocumentoString = EspacioEntreMayusculas(tipoDoc.ToString());
            ViewBag.TipoDocumento = tipoDoc;
            ViewBag.NumeroDocumento = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.TieneDatosExportacion = tieneDatosExportacion;
            SetearDatosPestanias(recorridoId, tipoDoc, numeroDoc, patente);
            if(recorrido?.Centro?.Id != 5 || recorrido?.TipoVehiculo == TipoVehiculo.Tren)
            {
                var fotoSustentable = servicio.ObtenerFotoSustentable(recorrido.Centro.Id, recorrido.NumeroDocumentoIngreso, "sustentable");
                if(fotoSustentable != null && !(fotos.Fotos.Where(f => f.Foto != fotoSustentable.Foto && f.Fecha != fotoSustentable.Fecha).ToList().Count > 0))
                {
                    fotos.Fotos.Add(fotoSustentable);
                }
            }
            return View("Fotos", fotos);
        }

        public ActionResult Recorrido(int recorridoId, TipoDocumentoIngreso tipoDoc, string numeroDoc, string patente, bool filtrarPorTarjeta, bool tieneDatosExportacion = false)
        {
            var recorrido = servicio.ObtenerRecorrido(recorridoId);
            ViewBag.FiltrarPorTarjeta = filtrarPorTarjeta;
            ViewBag.TieneDatosExportacion = tieneDatosExportacion;
            if (recorrido != null)
            {
                ViewBag.Rechazado = recorrido.Rechazado;
                ViewBag.LogItems = servicio.ConsultaControlRecorridoLogActividad(recorrido.InstanciaWorkflow);
                ViewBag.WorkflowDescripcion = recorrido.Workflow.Descripcion;
                ViewBag.TipoDocumentoString = EspacioEntreMayusculas(tipoDoc.ToString());
                ViewBag.TipoDocumento = tipoDoc;
                ViewBag.NumeroDocumento = recorrido.NumeroDocumentoIngreso;
                ViewBag.Patente = recorrido.Patente;
            }
            ViewBag.TipoDocumentoString = EspacioEntreMayusculas(tipoDoc.ToString());
            ViewBag.TipoDocumento = tipoDoc;
            SetearDatosPestanias(recorridoId, tipoDoc, numeroDoc, patente);
            return View();
        }

        public ActionResult DatosExportacion(int recorridoId, TipoDocumentoIngreso tipoDoc, string numeroDoc, string patente, bool filtrarPorTarjeta, bool rechazado, bool tieneDatosExportacion)
        {
            ViewBag.Rechazado = rechazado;
            ViewBag.RecorridoId = recorridoId;
            ViewBag.Patente = patente;
            ViewBag.TieneDatosExportacion = tieneDatosExportacion;
            ViewBag.TipoDocumentoString = EspacioEntreMayusculas(tipoDoc.ToString());
            ViewBag.TipoDoc = tipoDoc;
            ViewBag.NumeroDocumento = numeroDoc;
            ViewBag.FiltrarPorTarjeta = filtrarPorTarjeta;

            ViewBag.Nacionalidades = servicio.ListarPaises().ToSelectList(dto => dto.Id.ToString(CultureInfo.InvariantCulture), dto => dto.Descripcion).OrderBy(s => s.Text);
            ViewBag.Firmas = servicio.ListarFirmas().ToSelectList(dto => dto.Id.ToString(CultureInfo.InvariantCulture), dto => dto.RazonSocial);

            IngresoDeDatosDeExportacionDto datosExportacionDto = servicio.ObtenerIngresoDeDatosDeExportacionPorRecorrido(recorridoId);
            return View(datosExportacionDto);
        }

        [HttpPost]
        public ActionResult DatosExportacion(IngresoDeDatosDeExportacionDto dto)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(dto.InstanciaWorkflow);

            ViewBag.Rechazado = recorrido.Rechazado;
            ViewBag.RecorridoId = recorrido.Id;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.TieneDatosExportacion = true;
            ViewBag.TipoDocumentoString = EspacioEntreMayusculas(recorrido.TipoDocumentoIngreso.ToString());
            ViewBag.TipoDoc = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumento = recorrido.NumeroDocumentoIngreso;
            ViewBag.FiltrarPorTarjeta = dto.FiltrarPorTarjeta;

            ViewBag.Nacionalidades = servicio.ListarPaises().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion).OrderBy(s => s.Text);
            ViewBag.Firmas = servicio.ListarFirmas().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.RazonSocial);
            ModelState.Remove("");
            if (ModelState.IsValid)
            {
                servicioComandos.Ejecutar(new ModificarIngresoDatosExportacion { Dto = dto });
            }
            dto = servicio.ObtenerIngresoDeDatosDeExportacionPorRecorrido(recorrido.Id);

            return View(dto);
        }

        [AjaxOnly]
        [ActionName("Listar")]
        [DatosUsuario]
        public ActionResult Listar(ModificarDocumentoDeIngresoDto model, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "FechaInicio", DirOrden dirOrden = DirOrden.Desc, bool soloLectura = true)
        {
            ActualizarCookie(model);
            ListarConsulta(datosUsuario.CentroId, pagina, ordenarPor, dirOrden, model);

            ViewBag.FiltrarPorTarjeta = model.FiltrarPorTarjeta;
            ViewBag.SoloLectura = soloLectura;

            if (!((ListaPaginada<RecorridoDto>)ViewBag.Items).Any())
            {
                ViewBag.Error = model.FiltrarPorTarjeta ? Textos.Error_TarjetaNoASignada : Textos.Error_NoResultados;
            }
            return View("Listar");
        }

        private void ListarConsulta(int centroId, int pagina, string ordenarPor, DirOrden dirOrden, ModificarDocumentoDeIngresoDto model)
        {
            var paginacion = new Paginacion(
            ordenarPor,
            dirOrden,
            pagina,
            10);
            model.NumeroDocumentoIngreso = string.IsNullOrEmpty(model.NumeroDocumentoIngreso) ? string.Empty : model.NumeroDocumentoIngreso;
            ViewBag.Items = servicio.ListarPaginadoRecorridosPorModificarDocumentoDeIngreso(centroId, paginacion, model);
        }

        public ActionResult Seleccionar(int id, bool filtrarPorTarjeta = false, bool soloLectura = true, bool actualizarCookie = false)
        {
            var tipoDocumentoIngreso = servicio.ObtenerTipoDocumentoIngresoPorRecorrido(id);
            var tieneDatosExportacion = servicio.ObtenerIngresoDeDatosDeExportacionPorRecorrido(id) != null;

            return RedirectToAction("DocumentoOrigen", new
            {
                recorridoId = id,
                tipoDoc = tipoDocumentoIngreso,
                soloLectura,
                filtrarPorTarjeta,
                actualizarCookie,
                tieneDatosExportacion = tieneDatosExportacion
            });
        }

        public void SetearDatosPestanias(int recorridoId, TipoDocumentoIngreso tipoDoc, string numeroDoc, string patente)
        {
            ViewBag.RecorridoId = recorridoId;
            ViewBag.TipoDoc = tipoDoc;
            ViewBag.NumeroDoc = numeroDoc;
            ViewBag.Patente = patente;
        }

        private string EspacioEntreMayusculas(string palabra)
        {
            return String.Concat(palabra.ToString().Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }
    }
}
