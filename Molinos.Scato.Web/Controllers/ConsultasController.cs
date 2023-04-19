using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.Seguridad;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    public class ConsultasController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ConsultasController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult BuscarTransportista(string term)
        {
            var transporstista = servicio.BuscarTransportista(term);
            return transporstista != null ? Json(new { label = transporstista.Cuit + " - " + transporstista.RazonSocial, transporstista.Id, transporstista.RazonSocial }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarTransportistas(string term)
        {
            log.Info("Comienza la búsqueda de Transportistas");
            var transporstistas = servicio.BuscarTransportistas(term);
            log.Info("Finaliza la búsqueda de Transportistas");
            return Json(transporstistas.Select(s => new { label = s.Cuit + " - " + s.RazonSocial, s.Id, s.RazonSocial }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarProcedenciaUnica(string term)
        {
            var procedencia = servicio.BuscarProcedencia(term);
            return procedencia != null ? Json(new { label = procedencia.CodigoAfip + " - " + procedencia.Descripcion + "(" + procedencia.ProvinciaDesc + ")", procedencia.Id, procedencia.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarProcedencias(string term)
        {
            log.Info("Comienza la búsqueda de Procedencia");
            var procedencia = servicio.BuscarProcedencias(term);
            log.Info("Finaliza la búsqueda de Procedencia");
            return Json(procedencia.Select(s => new { label = s.CodigoAfip + " - " + s.Descripcion + "(" + s.ProvinciaDesc + ")", s.Id, s.Descripcion }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarProveedor(string term, TiposProveedor tipo)
        {
            var proveedor = servicio.BuscarProveedor(term, tipo);
            return proveedor != null ? Json(new { label = proveedor.Cuil + " - " + proveedor.Descripcion, proveedor.Id, proveedor.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarProveedores(string term, TiposProveedor tipo)
        {
            log.Info("Comienza la búsqueda de Proveedores: {0}", term);
            var proveedores = servicio.BuscarProveedores(term, tipo);
            log.Info("Finaliza la búsqueda de Proveedores: {0}", term);
            return Json(proveedores.Select(s => new { label = s.Cuil + " - " + s.Descripcion, s.Id, s.Descripcion }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarProveedoresConBocaDestino(string term)
        {
            log.Info("Comienza la búsqueda de Proveedores con Boca de Destino");
            var proveedores = servicio.ListarProveedoresConBocaDestino(term);
            log.Info("Finaliza la búsqueda de Proveedores con Boca de Destino");
            return Json(proveedores.Select(s => new { label = s.Cuil + " - " + s.Descripcion, s.Id, s.Descripcion }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarProveedorConBocaDestinoUnico(string term)
        {
            var proveedor = servicio.ObtenerProveedorConBocaDestino(term);
            return proveedor != null ? Json(new { label = proveedor.Cuil + " - " + proveedor.Descripcion, proveedor.Id, proveedor.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarCentroUnico(string term)
        {
            var centro = servicio.BuscarCentro(term);
            return centro != null ? Json(new { label = centro.Descripcion, centro.Id, centro.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarCentros(string term)
        {
            log.Info("Comienza la búsqueda de Centros");
            var centros = servicio.BuscarCentros(term);
            log.Info("Finaliza la búsqueda de Centros");
            return Json(centros.Select(s => new { label = s.Descripcion, s.Id, s.Descripcion }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarCentrosBodega(string term)
        {
            var centros = servicio.BuscarCentrosBodega(term);
            return Json(centros.Select(s => new { label = s.NumeroINV + "-" + s.Descripcion, s.Id, s.NumeroINV }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarCentroBodega(string term)
        {
            var centro = servicio.BuscarCentroBodega(term);
            return centro != null
                       ? Json(new { label = centro.NumeroINV + "-" + centro.Descripcion, centro.Id, centro.NumeroINV },
                              JsonRequestBehavior.AllowGet)
                       : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarEntregador(string term)
        {
            var entregador = servicio.BuscarEntregador(term);
            return entregador != null ? Json(new { label = entregador.Cuil + " - " + entregador.RazonSocial, entregador.Id, entregador.RazonSocial }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarEntregadores(string term)
        {
            log.Info("Comienza la búsqueda de Entregadores");
            var entregadores = servicio.BuscarEntregadores(term);
            log.Info("Finaliza la búsqueda de Entregadores");
            return Json(entregadores.Select(s => new { label = s.Cuil + " - " + s.RazonSocial, s.Id, s.RazonSocial }), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarMaterial(string term, DatosUsuario datosUsuario)
        {
            MaterialDto material;
            if ("TODOS".Contains(term.ToUpper()))
            {
                material = new MaterialDto { Descripcion = "Todos", Id = 0 };
            }
            else
            {
                material = servicio.BuscarMaterial(datosUsuario.CentroId, term);
            }
            return material != null ? Json(new { label = material.Descripcion, material.Id, material.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarMateriales(string term, string tipoCalle, DatosUsuario datosUsuario)
        {
            var materiales = servicio.BuscarMaterialesPorCentro(datosUsuario.CentroId, term, tipoCalle: tipoCalle);
            return Json(materiales.Select(s => new { label = s.MaterialDesc, Id = s.MaterialId, s.MaterialDesc }), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarMaterialPorCentro(string term, DatosUsuario datosUsuario)
        {
            var material = servicio.BuscarMaterialPorCentro(datosUsuario.CentroId, term);
            return material != null ? Json(new { label = material.MaterialDesc, material.Id, material.MaterialDesc }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarMaterialesPorCentro(string term, DatosUsuario datosUsuario)
        {
            var materiales = servicio.BuscarMaterialesPorCentro(datosUsuario.CentroId, term);
            return Json(materiales.Select(s => new { label = s.MaterialDesc, Id = s.Id, s.MaterialDesc }), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarCaracteristicasDeCalidadPorMaterial(int materialId, DatosUsuario datosUsuario)
        {
            var caracteristicasDeCalidad = servicio.ListarCaracteristicasDeCalidadPorMaterial(materialId, datosUsuario.CentroId);
            return Json(caracteristicasDeCalidad.Select(caracteristicaDeCalidad => new { label = caracteristicaDeCalidad.DescripcionCorta, Id = caracteristicaDeCalidad.Id, caracteristicaDeCalidad.DescripcionCorta }), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarCaracteristicaDeCalidadPorId(int id)
        {
            var caracteristicaDeCalidad = servicio.ObtenerCaracteristicaDeCalidad(id);
            return caracteristicaDeCalidad != null
                ? Json(new { id = caracteristicaDeCalidad.Id, nombre = caracteristicaDeCalidad.DescripcionCorta, caladoMaximo = caracteristicaDeCalidad.CaladoMaximo, caladoMinimo = caracteristicaDeCalidad.CaladoMinimo }, JsonRequestBehavior.AllowGet)
                : Json("", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarVinedo(string term)
        {
            var vinedo = servicio.BuscarVinedo(term);
            return vinedo != null ? Json(new { label = vinedo.NumeroINV + "-" + vinedo.Descripcion, vinedo.Id, vinedo.NumeroINV }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarVinedos(string term)
        {
            var vinedos = servicio.BuscarVinedos(term);
            return Json(vinedos.Select(s => new { label = s.NumeroINV + "-" + s.Descripcion, Id = s.Id, s.NumeroINV }), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarProveedorVinedoTercero(string term)
        {
            var proveedor = servicio.BuscarProveedorVinedoTercero(term);
            return proveedor != null
                       ? Json(new { label = proveedor.Cuil + "-" + proveedor.Descripcion, proveedor.Id },
                              JsonRequestBehavior.AllowGet)
                       : Json("", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarProveedoresVinedosTerceros(string term)
        {
            var proveedores = servicio.BuscarProveedoresVinedosTerceros(term);
            return Json(proveedores.Select(s => new { label = s.Cuil + "-" + s.Descripcion, Id = s.Id }),
                        JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarVinedosPropios(string term)
        {
            var vinedo = servicio.BuscarVinedosPropios(term);
            return Json(vinedo.Select(s => new { label = s.NumeroINV + "-" + s.Descripcion, s.Id, s.NumeroINV }),
                        JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarVinedoPropio(string term)
        {
            var vinedo = servicio.BuscarVinedoPropio(term);
            return vinedo != null
                       ? Json(new { label = vinedo.NumeroINV + "-" + vinedo.Descripcion, vinedo.Id, vinedo.NumeroINV },
                              JsonRequestBehavior.AllowGet)
                       : Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerProveedoresSap(string term, TiposProveedor tipo)
        {
            log.Info("Comienza la búsqueda de Proveedores Sap");
            servicioComandos.Ejecutar(new SincronizarProveedores { RetornarResultado = false, Cuit = term, CargaMasiva = false });
            log.Info("Fianliza la búsqueda de Proveedores Sap");
            var proveedores = servicio.BuscarProveedoresPorCuit(MascaraCuit(term), tipo);
            return Json(proveedores.Select(s => new { label = s.Cuil + " - " + s.Descripcion, s.Id, s.Descripcion }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerBocasDestino(int? proveedorId)
        {
            log.Info("Comienza la búsqueda de Bocas Destino");
            if (proveedorId != 0 && proveedorId != null)
            {
                var bocas =
                    servicio.ListarBocasDestino().Where(b => b.ProveedorId == proveedorId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombreBocaDeDestino);
                log.Info("Finaliza la búsqueda de Bocas Destino");
                return Json(bocas, JsonRequestBehavior.AllowGet);
            }
            log.Info("Comienza la búsqueda de Bocas Destino");
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarChoferes(string term)
        {
            var choferFiltro = new ChoferFiltro { Cuil = term };
            var choferes = servicio.BuscarChoferes(choferFiltro);
            return Json(choferes.Select(s => new { label = s.Cuil, s.Id, s.Cuil, s.NumeroDeDocumento, s.TipoDocumentoIdentidadId, s.Nombre, s.Apellido }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarChoferUnico(string term)
        {
            var choferFiltro = new ChoferFiltro { Cuil = term };
            var choferes = servicio.BuscarChoferes(choferFiltro);
            return choferes.Count == 1 ? Json(new { label = choferes.First().Cuil, choferes.First().Id, choferes.First().Cuil, choferes.First().NumeroDeDocumento, choferes.First().TipoDocumentoIdentidadId, choferes.First().Nombre, choferes.First().Apellido }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarClientes(string term)
        {
            var proveedores = servicio.BuscarClientes(term);
            return Json(proveedores.Select(s => new { label = s.CodigoSap + " - " + s.Descripcion, s.Id, s.Descripcion }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarCliente(string term)
        {
            var cliente = servicio.BuscarCliente(term);
            return cliente != null ? Json(new { label = cliente.CodigoSap + " - " + cliente.Descripcion, cliente.Id, cliente.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerClientesSap(string term)
        {
            log.Info("Comienza la búsqueda de Clientes Sap");
            servicioComandos.Ejecutar(new SincronizarClientes { RetornarResultado = false, Cuit = term, CargaMasiva = false });
            log.Info("Fianliza la búsqueda de Clientes Sap");
            var clientes = servicio.BuscarClientesPorCuit(MascaraCuit(term));
            return Json(clientes.Select(s => new { label = s.CodigoSap + " - " + s.Descripcion, s.Id, s.Descripcion }), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerPuestoDeTrabajo(string nombrePc, DatosUsuario datosUsuario)
        {
            var puesto = servicio.ObtenerPuestoDeTrabajoPorNombrePc(nombrePc, datosUsuario.CentroId) ?? new PuestoDeTrabajoDto();
            return Json(puesto, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult RedireccionarAListaAutomatizada(string nombrePc, DatosUsuario datosUsuario)
        {
            var redireccionar = servicio.RedireccionarAListaAutomatizada(nombrePc, datosUsuario.CentroId);

            return Json(redireccionar, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult RedireccionarABalanzaAutomatizada()
        {
            var redireccionar = PermisosHelper.Is(PermisosScato.BalanzaAutomatica);

            return Json(redireccionar, JsonRequestBehavior.AllowGet);
        }

        protected static string MascaraCuit(string entrada)
        {
            if (entrada.Length != 11)
            {
                return entrada;
            }
            return entrada.Substring(0, 2) + "-" + entrada.Substring(2, 8) + "-" + entrada.Substring(10);
        }

        [DatosUsuario]
        public ActionResult BuscarDescripcionCortaMaterial(string term, DatosUsuario datosUsuario)
        {
            var material = servicio.BuscarDescripcionCortaMaterial(datosUsuario.CentroId, term);
            return material != null ? Json(new { label = material, Id = 1, Descripcion = material }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarDescripcionesCortasMateriales(string term, DatosUsuario datosUsuario)
        {
            var materiales = servicio.BuscarDescripcionMaterialesPorCentro(datosUsuario.CentroId, term);
            return Json(materiales.Select(s => new { label = s, Id = 1, MaterialDesc = s }), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarKmPorProveedor(int clienteId, DatosUsuario datosUsuario)
        {
            var kmPorProveedor = servicio.ListarKmPorProveedorYCentro(clienteId, datosUsuario.CentroId);
            return Json(kmPorProveedor.Select(s => new { kmRecorrer = s.KmARecorrer, localidadDescripcion = s.LocalidadDescripcion, localidadDestinoId = s.LocalidadId }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult ObtenerTiposVehiculo(DatosUsuario datosUsuario, bool conTren = false)
        {
            var tiposVehiculo = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(datosUsuario.CentroId);
            if (!conTren)
            {
                tiposVehiculo = tiposVehiculo.Where(s => s.TipoVehiculo != TipoVehiculo.Tren).ToList();
            }
            return Json(tiposVehiculo.Select(s => new { netoMaximo = s.PesoNetoMaxPlanta, brutoMaximoIngreso = s.PesoMaxIngreso, brutoMaximoEgreso = s.PesoMaxEgreso, tipoVehiculoText = s.TipoVehiculo.DisplayText(), netoMinimo = s.PesoNetoMinimo, tipoVehiculoValue = (int)s.TipoVehiculo }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarVapor(string term)
        {
            var vapor = servicio.BuscarVapor(term);
            return vapor != null ? Json(new { label = vapor.Nombre, vapor.Id }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarVapores(string term)
        {
            log.Info("Comienza la búsqueda de Vapores");
            var vapores = servicio.BuscarVapores(term);
            log.Info("Finaliza la búsqueda de Vapores");
            return Json(vapores.Select(s => new { label = s.Nombre, s.Id }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarBodega(string term)
        {
            var bodega = servicio.BuscarBodega(term);
            return bodega != null ? Json(new { label = bodega.Nombre, bodega.Id }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarBodegas(string term)
        {
            log.Info("Comienza la búsqueda de Bodegas");
            var bodegas = servicio.BuscarBodegas(term);
            log.Info("Finaliza la búsqueda de Bodegas");
            return Json(bodegas.Select(s => new { label = s.Nombre, s.Id }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarExportador(string term)
        {
            var exportador = servicio.BuscarExportador(term);
            return exportador != null ? Json(new { label = exportador.Nombre, exportador.Id }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarExportadores(string term)
        {
            log.Info("Comienza la búsqueda de Exportadores");
            var exportadores = servicio.BuscarExportadores(term);
            log.Info("Finaliza la búsqueda de Exportadores");
            return Json(exportadores.Select(s => new { label = s.Nombre, s.Id }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarDestino(string term)
        {
            var destino = servicio.BuscarDestino(term);
            return destino != null ? Json(new { label = destino.Nombre, destino.Id }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarDestinos(string term)
        {
            log.Info("Comienza la búsqueda de Exportadores");
            var destinos = servicio.BuscarDestinos(term);
            log.Info("Finaliza la búsqueda de Exportadores");
            return Json(destinos.Select(s => new { label = s.Nombre, s.Id }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarMaterialPuerto(string term)
        {
            var material = servicio.BuscarMaterialPuerto(term);
            return material != null ? Json(new { label = material.Descripcion, material.Id }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarMaterialesPuerto(string term)
        {
            log.Info("Comienza la búsqueda de Exportadores");
            var materiales = servicio.BuscarMaterialesPuerto(term);
            log.Info("Finaliza la búsqueda de Exportadores");
            return Json(materiales.Select(s => new { label = s.Descripcion, s.Id }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarAlmacen(string term)
        {
            var almacen = servicio.BuscarAlmacenPuerto(term);
            return almacen != null ? Json(new { label = almacen.Descripcion, almacen.Id }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarAlmacenes(string term)
        {
            log.Info("Comienza la búsqueda de Almacenes");
            var almacenes = servicio.BuscarAlmacenesPuerto(term);
            log.Info("Finaliza la búsqueda de Almacenes");
            return Json(almacenes.Select(s => new { label = s.Descripcion, s.Id }), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult ListarMaterialesPorCliente(DatosUsuario datosUsuario, int workflowId, int clienteId)
        {
            var materiales = servicio.ListarMaterialesPorWorkflowCliente(workflowId, datosUsuario.CentroId, clienteId).ToSelectList(f => f.MaterialId.ToString(CultureInfo.InvariantCulture), f => f.MaterialDesc);

            return Json(materiales, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarMaterialesConTodos(string term, DatosUsuario datosUsuario)
        {
            var materiales = servicio.BuscarMaterialesPorCentro(datosUsuario.CentroId, term);
            if ("TODOS".Contains(term.ToUpper()))
            {
                materiales = materiales.ToList();
                materiales.Add(new MaterialPorCentroDto { MaterialDesc = "Todos", MaterialId = 0, CentroId = datosUsuario.CentroId });
            }
            return Json(materiales.Select(s => new { label = s.MaterialDesc, Id = s.MaterialId, s.MaterialDesc }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClienteBloqueado(int clienteId)
        {
            var cliente = servicio.ObtenerCliente(clienteId);
            return cliente != null ? Json(new { bloqueado = cliente.Bloqueado }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerMensajeFijo(DatosUsuario datoUsuario)
        {
            var mensajesFijo = servicio.ObtenerNotificacionAplicacion(datoUsuario.NombreUsuario);
            return Json(mensajesFijo, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult ObtenerRamalFerroviario(DatosUsuario datosUsuario, bool conTren = false)
        {
            var ramalFerroviario = servicio.ListarRamalFerroviario();
            return Json(ramalFerroviario.Select(s => new { label = s.Descripcion, s.CodigoAfip }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerNombrePc(DatosUsuario datosUsuario)
        {
            var nombrePc = System.Security.Claims.ClaimsPrincipal.
                Current.FindFirst(x => x.Type == "UserComputerName")?.Value;
            return Json(nombrePc, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerCPEAutomotorDG(DatosUsuario datosUsuario, long numeroCTG)
        {
                var result = servicioComandos.Ejecutar(new ConsultarCPEAutomotorDG
                {
                    CentroId = datosUsuario.CentroId,
                    Usuario = datosUsuario.NombreUsuario,
                    NumeroCTG = numeroCTG
                });

                return Json(result, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerPlantasDGPorClienteId(DatosUsuario datosUsuario, int clienteId, string clienteCuit = null)
        {
            var result = new Resultado();
            var cuit = "0";
            if(!string.IsNullOrEmpty(clienteCuit))
            {
                cuit = clienteCuit;
            } else
            {
                var cliente = servicio.ObtenerCliente(clienteId);
                if(cliente == null)
                {
                    result.Errores.Add("2", "No existe el cliente ingresado.");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                cuit = cliente.Cuit.Replace("-", string.Empty);
            }
            result = servicioComandos.Ejecutar(new ConsultarPlantasDG
            {
                CentroId = datosUsuario.CentroId,
                Cuit = long.Parse(cuit)
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerDomiciliosDG(DatosUsuario datosUsuario, int clienteId, string clienteCuit = null)
        {
            var result = new Resultado();
            var cuit = "0";
            if (!string.IsNullOrEmpty(clienteCuit))
            {
                cuit = clienteCuit;
            } else
            {
                var cliente = servicio.ObtenerCliente(clienteId);
                if (cliente == null)
                {
                    result.Errores.Add("2", "No existe el cliente ingresado.");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                cuit = cliente.Cuit.Replace("-", string.Empty);
            }
            result = servicioComandos.Ejecutar(new ConsultarDomiciliosDG
            {
                CentroId = datosUsuario.CentroId,
                Cuit = long.Parse(cuit)
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult ObtenerMaterial(short codigoPadre, short codigoDerivadoGranario)
        {
            var material = servicio.ObtenerMaterialDerivadoGranario(codigoPadre, codigoDerivadoGranario);
            return material != null ? Json(new { label = material.Descripcion, material.Id, material.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult ObtenerProcedenciaDG(int plantaDG)
        {
            var domicilio = servicio.ObtenerDomicilioDG(plantaDG);
            return domicilio != null ? Json(new { label = domicilio.Descripcion, domicilio.Id, domicilio.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }
    }
}