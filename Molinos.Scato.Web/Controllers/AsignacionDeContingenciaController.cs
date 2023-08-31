using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AsignacionDeContingencia)]
    public class AsignacionDeContingenciaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;

        public AsignacionDeContingenciaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador servicioOrquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "NombrePuesto", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View();
        }
        private void ListQuery(string filtro, int centroId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoPuestosDeTrabajoContingencia("Garita Ingreso", centroId, paginacion);
            ViewBag.Nirs = servicio.ListarNirsPorCentro(centroId).ToList();
            ViewBag.Centro = servicio.ObtenerCentro(centroId);
            ViewBag.Humedimetros = servicio.ListarHumedimetrosPorCentro(centroId).ToList();
            ViewBag.Balanzas = servicio.ListarTodasLasBalanzasActivas(centroId).ToList();

            ViewBag.ImpresionTicketSalida = !servicio.ImprimeTicketSalida(centroId);

            var puestos = servicio.ListarPuestosDeTrabajoPorCentro(centroId);
            var puestoGranos = puestos.Where(x => x.NombrePuesto == "Ingreso Planta").FirstOrDefault();

            var puestoNoGranos = puestos.Where(x => x.NombrePuesto.Contains("Ingreso Planta No Granos")).FirstOrDefault();
            ViewBag.PuestoGranos = puestoGranos;
            ViewBag.PuestoNoGranos = puestoNoGranos;
            ViewBag.ContingenciaGranos = servicio.EsPuestoEnContingencia(puestoGranos!=null?puestoGranos.Id:0, true);
            ViewBag.ContingenciaNoGranos = servicio.EsPuestoEnContingencia(puestoNoGranos!=null?puestoNoGranos.Id:0, false);
        }

        [DatosUsuario]
        public JsonResult ModificarModalidad(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerPuestoDeTrabajo(id);
                var activado = true;
                if (!aModificar.PidePatente && aModificar.ImprimeTarjetaDeAcceso)
                {
                    aModificar.PidePatente = true;
                    aModificar.ImprimeTarjetaDeAcceso = false;
                    activado = false;
                }
                else
                {
                    aModificar.PidePatente = false;
                    aModificar.ImprimeTarjetaDeAcceso = true;
                }

                var comando = new ModificarPuestoDeTrabajo { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia($"Solo imprimir etiqueta en puesto { aModificar.NombrePuesto }", datosUsuario.NombreUsuario, activado, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult SinAfip(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerPuestoDeTrabajo(id);
                aModificar.SinAfip = !aModificar.SinAfip;

                //REMOVIDO POR REQUERIMINETO DEL TICKET SCT310-1607
                //if (aModificar.SinAfip == true) 
                //{
                //    var aCentroModificar = servicio.ObtenerCentro(datosUsuario.CentroId);
                //    CambiarInformarCircular(datosUsuario, aCentroModificar, false, motivo);
                //}

                var comando = new ModificarPuestoDeTrabajo { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia($"No obtener datos de AFIP en puesto { aModificar.NombrePuesto }", datosUsuario.NombreUsuario, aModificar.SinAfip, motivo);

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult SinCupo(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerPuestoDeTrabajo(id);
                aModificar.SinCupo = !aModificar.SinCupo;

                var comando = new ModificarPuestoDeTrabajo { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia($"No validar Cupo en SAP en puesto { aModificar.NombrePuesto }", datosUsuario.NombreUsuario, aModificar.SinCupo, motivo);

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult SinFotoCartaPorte(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerPuestoDeTrabajo(id);
                aModificar.SinFotoCartaPorte = !aModificar.SinFotoCartaPorte;

                var comando = new ModificarPuestoDeTrabajo { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia($"No tomar foto de CP en puesto { aModificar.NombrePuesto }", datosUsuario.NombreUsuario, aModificar.SinFotoCartaPorte, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult ImprimeCartaPorte(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerPuestoDeTrabajo(id);
                aModificar.ImprimeCartaPorte = !aModificar.ImprimeCartaPorte;

                var comando = new ModificarPuestoDeTrabajo { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia($"No imprimir CP en mesa en puesto { aModificar.NombrePuesto }", datosUsuario.NombreUsuario, aModificar.ImprimeCartaPorte, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult ImprimeTarjetaDeAcceso(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerPuestoDeTrabajo(id);
                aModificar.ImprimeTarjetaDeAcceso = !aModificar.ImprimeTarjetaDeAcceso;

                var comando = new ModificarPuestoDeTrabajo { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia($"Imprimir etiqueta tarjeta en puesto { aModificar.NombrePuesto }", datosUsuario.NombreUsuario, aModificar.ImprimeTarjetaDeAcceso, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }

        [DatosUsuario]
        public JsonResult TomarFotoCartaDePorteEnCentro(DatosUsuario datosUsuario, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerCentro(datosUsuario.CentroId);
                aModificar.TomarFotoEnMesa = !aModificar.TomarFotoEnMesa;

                var comando = new ModificarCentro { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia("No tomar foto de CP en Mesa de Entrada", datosUsuario.NombreUsuario, aModificar.TomarFotoEnMesa, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult InformarArriboACircularEnCentro(DatosUsuario datosUsuario, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerCentro(datosUsuario.CentroId);
                CambiarInformarCircular(datosUsuario, aModificar, !aModificar.InformaCircular, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }

        private void CambiarInformarCircular(DatosUsuario datosUsuario, CentroDto aModificar, bool informar, string motivo)
        {
            aModificar.InformaCircular = informar;

            var comando = new ModificarCentro { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

            var resultado = servicioComandos.Ejecutar(comando);
            RegistracionContingencia("Informar arribos de camiones a Circular", datosUsuario.NombreUsuario, aModificar.InformaCircular, motivo);
        }

        [DatosUsuario]
        public JsonResult DescargaCartaPortePorCtg(DatosUsuario datosUsuario, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerCentro(datosUsuario.CentroId);
                aModificar.DescargaCartaPortePorCtg = !aModificar.DescargaCartaPortePorCtg;

                var comando = new ModificarCentro { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia("No obtener datos por CTG", datosUsuario.NombreUsuario, aModificar.DescargaCartaPortePorCtg, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult SolicitaConfirmarCTG(DatosUsuario datosUsuario, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerCentro(datosUsuario.CentroId);
                aModificar.SolicitaConfirmarCTG = !aModificar.SolicitaConfirmarCTG;

                var comando = new ModificarCentro { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia("Baja CTG manual", datosUsuario.NombreUsuario, aModificar.SolicitaConfirmarCTG, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult ValidarCupo(DatosUsuario datosUsuario, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerCentro(datosUsuario.CentroId);
                aModificar.ValidarCupo = !aModificar.ValidarCupo;

                var comando = new ModificarCentro { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia("No validar Cupo en SAP", datosUsuario.NombreUsuario, aModificar.ValidarCupo, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult EncolaBajaCtgAutomatico(DatosUsuario datosUsuario, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerCentro(datosUsuario.CentroId);
                aModificar.EncolaBajaCtgAutomatico = !aModificar.EncolaBajaCtgAutomatico;

                var comando = new ModificarCentro { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia("Encola Baja CTG Automatico", datosUsuario.NombreUsuario, aModificar.ValidarCupo, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult NirsManual(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerNirs(id);
                var model = new NirsModificacionModalidadDto
                {
                    Fecha = DateTime.Now,
                    NombreUsuarioResponsable = datosUsuario.NombreUsuario,
                    Modalidad = aModificar.Modalidad == Modalidad.Manual ? Modalidad.Automática : Modalidad.Manual,
                    Motivo = $"Pantalla de contingencia, usuario: {datosUsuario.NombreUsuario }",
                    NirsId = aModificar.Id,
                    NirsDescripcion = aModificar.Descripcion
                };

                var resultado = servicioComandos.Ejecutar(new CrearNirsModificarModalidad { Dto = model });
                RegistracionContingencia($"Modalidad {model.Modalidad.DisplayText()} de Nirs {aModificar.DescripcionCorta}", datosUsuario.NombreUsuario,
                    aModificar.Modalidad == Modalidad.Manual, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult HumedimetroManual(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerHumedimetro(id);
                var model = new HumedimetroModificacionModalidadDto
                {
                    Fecha = DateTime.Now,
                    NombreUsuarioResponsable = datosUsuario.NombreUsuario,
                    Modalidad = aModificar.Modalidad == Modalidad.Manual ? Modalidad.Automática : Modalidad.Manual,
                    Motivo = $"Pantalla de contingencia, usuario: {datosUsuario.NombreUsuario }",
                    HumedimetroId = aModificar.Id,
                    HumedimetroDescripcion = aModificar.Descripcion
                };

                var resultado = servicioComandos.Ejecutar(new CrearHumedimetroModificarModalidad { Dto = model });
                RegistracionContingencia($"Modalidad {model.Modalidad.DisplayText()} de Humedimetro {aModificar.DescripcionCorta}", datosUsuario.NombreUsuario,
                    aModificar.Modalidad == Modalidad.Manual, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult BalanzaManual(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerBalanza(id);
                var model = new BalanzaModificacionModalidadDto
                {
                    Fecha = DateTime.Now,
                    NombreUsuarioResponsable = datosUsuario.NombreUsuario,
                    Modalidad = aModificar.Modalidad == Modalidad.Manual ? Modalidad.Automática : Modalidad.Manual,
                    Motivo = $"Pantalla de contingencia, usuario: {datosUsuario.NombreUsuario }",
                    BalanzaId = aModificar.Id,
                    BalanzaNombre = aModificar.Nombre
                };

                var resultado = servicioComandos.Ejecutar(new CrearBalanzaModificarModalidad { Dto = model });
                RegistracionContingencia($"Modalidad {model.Modalidad.DisplayText()} de Balanza {aModificar.NombreCorto}", datosUsuario.NombreUsuario,
                    aModificar.Modalidad == Modalidad.Manual, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }

        [DatosUsuario]
        public JsonResult ImpresionTicketDeSalida(DatosUsuario datosUsuario, string motivo)
        {
            try
            {

                var contingencia = !servicio.ImprimeTicketSalida(datosUsuario.CentroId);
                servicioComandos.Ejecutar(new ModificarTodoMaterialPorCentro
                {
                    MaterialPorCentroDto = new MaterialPorCentroDto { CentroId = datosUsuario.CentroId, ImprimeReciboMunicipal = contingencia },
                    Usuario = datosUsuario.NombreUsuario
                });


                RegistracionContingencia($"Impresión ticket de salida", datosUsuario.NombreUsuario, !contingencia, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        [DatosUsuario]
        public JsonResult NoAsignaCalleEnGaritaEntrada(DatosUsuario datosUsuario, int id, string motivo)
        {
            try
            {
                var aModificar = servicio.ObtenerPuestoDeTrabajo(id);
                aModificar.NoAsignaCalleEnGaritaEntrada = !aModificar.NoAsignaCalleEnGaritaEntrada;

                var comando = new ModificarPuestoDeTrabajo { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia($"No asignar calles en { aModificar.NombrePuesto }", datosUsuario.NombreUsuario, aModificar.ImprimeCartaPorte, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }

        [DatosUsuario]
        public JsonResult ContingenciaGranos(DatosUsuario datosUsuario, int id, bool contingencia, bool esGranos, string motivo)
        {
            try
            {
                if (contingencia)
                {
                    servicioComandos.Ejecutar(new ActivarContingenciaIngresoPlanta { Usuario = datosUsuario.NombreUsuario, PuestoId = id, Granos = esGranos });
                }
                else
                {
                    servicioComandos.Ejecutar(new DesactivarContingenciaIngresoPlanta { Usuario = datosUsuario.NombreUsuario, PuestoId = id, Granos = esGranos, CentroId = datosUsuario.CentroId });
                }
                var granos = esGranos ? "Granos" : "no Granos";
                RegistracionContingencia($"Habilitación ingreso planta por calle de { granos }", datosUsuario.NombreUsuario, contingencia, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
        private void RegistracionContingencia(string contingencia, string usuario, bool activado, string motivo)
        {
            try
            {
                RegistrarContingencia(contingencia, usuario, activado, motivo);
                EnvioMailContingencia(contingencia, usuario, activado);

            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        private void RegistrarContingencia(string contingencia, string usuario, bool activado, string motivo)
        {
            try
            {
                servicioComandos.Ejecutar(new CrearContingencia
                {
                    Dto = new ContingenciaDto
                    {
                        Activado = activado,
                        Fecha = DateTime.Now,
                        TipoContingencia = contingencia,
                        Usuario = usuario,
                        Motivo = motivo
                    }
                });
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        private void EnvioMailContingencia(string contingencia, string usuario, bool activado)
        {
            try
            {
                var usuarios = servicio.ObtenerUsuariosContingencia();
                var habilitado = activado ? "habilitado" : "deshabilitado";
                if (usuarios.Count > 0)
                {
                    servicioComandos.Ejecutar(new EnvioMail
                    {
                        Destinatarios = usuarios,
                        Titulo = $"Contingencia {contingencia}",
                        Cuerpo = string.Format(Textos.MailContingencia, contingencia, usuario, habilitado)
                    });
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        [DatosUsuario]
        public JsonResult ContingenciaAfipCpe(DatosUsuario datosUsuario, string motivo)
        {

            try
            {
                var aModificar = servicio.ObtenerCentro(datosUsuario.CentroId);
                aModificar.ContingenciaAfipCpe = !aModificar.ContingenciaAfipCpe;

                var comando = new ModificarCentro { Dto = aModificar, Usuario = datosUsuario.NombreUsuario };

                var resultado = servicioComandos.Ejecutar(comando);
                RegistracionContingencia("Conecta a AFIP para buscar Carta Porte Electronica", datosUsuario.NombreUsuario, aModificar.ContingenciaAfipCpe, motivo);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
