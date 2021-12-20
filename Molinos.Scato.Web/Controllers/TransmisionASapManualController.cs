using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.Conversiones;
using System.Text.RegularExpressions;
using System.ServiceModel;
using Molinos.Scato.Servicios.Conversiones.Impl;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.TransmisionASapManual)]
    public class TransmisionASapManualController : BaseController
    {

        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly WaybillManagementPODv2 servicioMonsanto;
        private readonly IServicioSapAsincronico servicioSapAsinc;
        private readonly IConversor conversor;

        public TransmisionASapManualController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, ZSDWS_SCATO servicioSap, WaybillManagementPODv2 servicioMonsanto, IServicioSapAsincronico servicioSapAsinc)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioSap = servicioSap;
            this.servicioMonsanto = servicioMonsanto;
            this.servicioSapAsinc = servicioSapAsinc;
            this.conversor = new ConversorAutoMapper();
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc, TipoDeServicio tipoDeServicio = TipoDeServicio.Sap)
        {

            if (ModelState.IsValid)
            {
                ViewBag.FuncionesSap = new List<SelectListItem>(new List<SelectListItem>
                {
                    new SelectListItem{Text ="Z1000", Value = "Z1000"},
                    new SelectListItem{Text ="Ingresos Egresos Fazones", Value = "IngresosEgresosFazones"},
                    new SelectListItem{Text ="Mov305", Value = "Mov305"},
                    new SelectListItem{Text ="Ajuste de Diferencias", Value = "AjustedeDiferencias"},
                    new SelectListItem{Text ="Egresos No Productivos", Value = "EgresosNoProductivos"},
                    new SelectListItem{Text ="MOV975", Value = "MOV975"},
                    new SelectListItem{Text ="Informar Cupo", Value = "InformarCupo"},
                    new SelectListItem{Text ="ZE7550", Value = "ZE7550"}

                });
            }
            return View();
        }

        public JsonResult ProcesarTransmision(string datos, string tipo)
        {
            Dictionary<string, string> resultado = new Dictionary<string, string>();
            foreach (string dato in datos.Split(';'))
            {
                if (dato.Contains(',') && !resultado.ContainsKey(dato.Split(',')[0]))
                {
                    var documento = dato.Split(',')[0];
                    var patente = dato.Split(',')[1];

                    log.Info("Se va a procesar la Carta de porte: " + documento);
                    var recorrido = servicio.ObtenerRecorridoPorNumeroDocumento(documento).FirstOrDefault(x => x.Terminado && !x.Rechazado && x.Patente == patente);
                    if (recorrido != null)
                    {
                        var addmethod = this.GetType().GetMethod(tipo);
                        object result = addmethod.Invoke(this, new object[] { recorrido });
                        resultado.Add(documento, result.ToString());
                    }
                    else
                    {
                        resultado.Add(documento, "Carta de porte: " + documento + " no existe, se encuentra rechazada, o el recorrido no se encuentra terminado");
                    }
                }
                else
                {
                    if (dato != "" && !resultado.ContainsKey(dato))
                    {
                        resultado.Add(dato, dato + " no es válido. Ingrese Documento,Patente");
                    }
                }
            }
            return Json(new { Mensaje = resultado.Values.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public string ZE7550(RecorridoDto recorrido)
        {
            try
            {
                var request = ObtenerRequestZE7550(recorrido.Id, servicio, servicioComandos);
                log.Info("request:" + (request != null).ToString());
                log.Info("ZE7550:" + (request.Z_SDMF_RFC_ZE7550 != null).ToString());
                var transmision = conversor.Convertir<Z_SDMF_RFC_ZE7550, Molinos.Scato.Dominio.Entidades.ZE7550TransmisionASap>(request.Z_SDMF_RFC_ZE7550);

                log.Info("guardando en la base");
                transmision.FuncionSap = FuncionSAP.ZE7550;
                transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
                transmision.Fecha = DateTime.Now;
                transmision.MensajeError = "";
                transmision.Estado = EstadoTransmisionASap.Error;

                servicioComandos.Ejecutar(new ActualizarZE7550TransmisionASap { Dto = transmision });
                log.Info("Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente");
                return "Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente";
            }
            catch (Exception e)
            {
                log.Error("Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso + ". " + e.Message);
                return "Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso;
            }

        }

        private Z_SDMF_RFC_ZE7550Request ObtenerRequestZE7550(int recorridoId, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {
            log.Info("ObtenerRequest\n");
            var resultado = new Resultado();
            Z_SDMF_RFC_ZE7550Request request = null;
            try
            {
                var recorrido = srvRepositorio.ObtenerRecorrido(recorridoId);
                var muestraEnvioACamara = srvRepositorio.ObtenerUltimaMuestraEnvioACamaraPorCaladoId(recorrido.Calado.Id);
                ConfigurationManager.AppSettings["SepararAlmacenSustentable"] = "false";
                var guid = recorrido.InstanciaWorkflow;

                var cartaPorte = srvRepositorio.ObtenerCartaPortePorInstanceId(guid);
                var calado = srvRepositorio.ObtenerCaladoPorGuid(guid);
                var vehiculo = recorrido.Vehiculo;
                var fechaEgreso = recorrido.FechaEgreso;
                var pesoTara = recorrido.PesoTara;
                var pesoBruto = recorrido.PesoBruto;
                var pesoNeto = recorrido.PesoNeto ?? 0;
                var fechaPesoNeto = recorrido.PesoNetoFecha;
                var fechaPesoBruto = recorrido.PesoBrutoFecha ?? new DateTime();
                var fechaPesoTara = recorrido.PesoTaraFecha ?? new DateTime();
                var camaraId = muestraEnvioACamara != null ? muestraEnvioACamara.CamaraId : 0;
                var camionRechazado = recorrido.Rechazado;
                var centroId = recorrido.Centro.Id;
                var instanceId = recorrido.InstanciaWorkflow;
                log.Info("Iniciando generación\n");
                var target = new ZE7550GenerarRequest();
                var host = WorkflowInvokerTest.Create(target);
                host.Extensions.Add(srvRepositorio);
                host.Extensions.Add(servicioComandos
                    );
                host.Extensions.Add(() => new ScatoPersistenceParticipant());

                host.InArguments.CartaPorte = cartaPorte;
                host.InArguments.Vehiculo = recorrido.Vehiculo;
                host.InArguments.FechaEgreso = fechaEgreso;
                host.InArguments.PesoTara = pesoTara;
                host.InArguments.PesoBruto = pesoBruto;
                host.InArguments.PesoNeto = pesoNeto;
                host.InArguments.FechaPesoTara = fechaPesoNeto;
                host.InArguments.FechaPesoBruto = fechaPesoBruto;
                host.InArguments.FechaPesoNeto = fechaPesoTara;
                host.InArguments.CamaraId = camaraId;
                host.InArguments.CamionRechazado = camionRechazado;
                host.InArguments.CentroId = centroId;
                host.InArguments.InstanceId = instanceId;


                var retorno = host.TestActivity();
                request = (Z_SDMF_RFC_ZE7550Request)retorno.First(f => f.Key == "Request").Value;
                var errores = (Resultado)retorno.First(f => f.Key == "Resultado").Value;
                foreach (var error in errores.Errores)
                {
                    log.Error(error.Key + " - " + error.Value);
                }
                log.Info("Request Generada\n");
            }
            catch (Exception e)
            {

                log.Info("ERRPR" + e.Message);
                log.Info("ERRPR" + e.StackTrace);
                resultado.Errores.Add("", e.Message);
            }
           ;
            return request;
        }


        public string Z1000(RecorridoDto recorrido)
        {
            try
            {
                var request = ObtenerRequest(recorrido.Id, servicio, servicioComandos);
                log.Info("request:" + (request != null).ToString());
                log.Info("requestFill_Z1000:" + (request.Fill_Z1000 != null).ToString());
                var transmision = conversor.Convertir<Fill_Z1000, Molinos.Scato.Dominio.Entidades.IngresosPorCompraDeGranosTransmisionASap>(request.Fill_Z1000);
                foreach (var recepcion in request.Fill_Z1000.RecepcionesYDespachosII.Select(recepcionYRedespacho => conversor.Convertir<ZMPES0020, RecepcionYRedespacho>(recepcionYRedespacho)))
                {
                    transmision.RecepcionesYDespachosII.Add(recepcion);
                }
                log.Info("guardando en la base");
                transmision.FuncionSap = FuncionSAP.IngresosPorCompraDeGranos;
                transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
                transmision.Fecha = DateTime.Now;
                transmision.MensajeError = "";
                transmision.Estado = EstadoTransmisionASap.Error;

                servicioComandos.Ejecutar(new ActualizarIngresosPorCompraDeGranosTransmisionASap { Dto = transmision });
                log.Info("Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente");
                return "Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente";
            }
            catch (Exception e)
            {
                log.Error("Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso + ". " + e.Message);
                return "Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso;
            }

        }


        private Fill_Z1000Request ObtenerRequest(int recorridoId, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {
            log.Info("ObtenerRequest\n");
            var resultado = new Resultado();
            Fill_Z1000Request request = null;
            try
            {
                var recorrido = srvRepositorio.ObtenerRecorrido(recorridoId);
                var muestraEnvioACamara = srvRepositorio.ObtenerUltimaMuestraEnvioACamaraPorCaladoId(recorrido.Calado.Id);
                ConfigurationManager.AppSettings["SepararAlmacenSustentable"] = "false";
                var guid = recorrido.InstanciaWorkflow;

                var cartaPorte = srvRepositorio.ObtenerCartaPortePorInstanceId(guid);
                var calado = srvRepositorio.ObtenerCaladoPorGuid(guid);
                var vehiculo = recorrido.Vehiculo;
                var fechaEgreso = recorrido.FechaEgreso;
                var pesoTara = recorrido.PesoTara;
                var pesoBruto = recorrido.PesoBruto;
                var pesoNeto = recorrido.PesoNeto.Value;
                var fechaPesoNeto = recorrido.PesoNetoFecha;
                var fechaPesoBruto = recorrido.PesoBrutoFecha;
                var fechaPesoTara = recorrido.PesoTaraFecha;
                var camaraId = muestraEnvioACamara != null ? muestraEnvioACamara.CamaraId : 0;
                var camionRechazado = recorrido.Rechazado;
                var centroId = recorrido.Centro.Id;
                var instanceId = recorrido.InstanciaWorkflow;
                log.Info("Iniciando generación\n");
                var target = new IngresosPorCompraDeGranosGenerarRequest();
                var host = WorkflowInvokerTest.Create(target);
                host.Extensions.Add(srvRepositorio);
                host.Extensions.Add(servicioComandos
                    );
                host.Extensions.Add(() => new ScatoPersistenceParticipant());

                host.InArguments.CartaPorte = cartaPorte;
                host.InArguments.Calado = calado;
                host.InArguments.Vehiculo = recorrido.Vehiculo;
                host.InArguments.FechaEgreso = fechaEgreso;
                host.InArguments.PesoTara = pesoTara;
                host.InArguments.PesoBruto = pesoBruto;
                host.InArguments.PesoNeto = pesoNeto;
                host.InArguments.FechaPesoTara = fechaPesoNeto;
                host.InArguments.FechaPesoBruto = fechaPesoBruto;
                host.InArguments.FechaPesoNeto = fechaPesoTara;
                host.InArguments.CamaraId = camaraId;
                host.InArguments.CamionRechazado = camionRechazado;
                host.InArguments.CentroId = centroId;
                host.InArguments.InstanceId = instanceId;

                var retorno = host.TestActivity();
                request = (Fill_Z1000Request)retorno.First(f => f.Key == "Request").Value;
                var errores = (Resultado)retorno.First(f => f.Key == "Resultado").Value;
                foreach (var error in errores.Errores)
                {
                    log.Error(error.Key + " - " + error.Value);
                }
                log.Info("Request Generada\n");
            }
            catch (Exception e)
            {

                log.Info("ERRPR" + e.Message);
                log.Info("ERRPR" + e.StackTrace);
                resultado.Errores.Add("", e.Message);
            }
           ;
            return request;
        }


        public string IngresosEgresosFazones(RecorridoDto recorrido)
        {
            try
            {
                var t = ObtenerIngresosEgresosFazones(recorrido.Id, servicio, servicioComandos);
                servicioComandos.Ejecutar(new ActualizarIngresosEgresosFazonesTransmisionASap { Dto = t });
                log.Info("Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente");
                return "Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente";
            }
            catch (Exception e)
            {
                log.Error("Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso + ". " + e.Message);
                return "Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso;
            }
        }

        private Dominio.Entidades.IngresosEgresosFazonesTransmisionASap ObtenerIngresosEgresosFazones(int recorridoId, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {
            var target = new IngresosEgresosFazonesGenerarRequest();
            var host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio);
            host.Extensions.Add(() => new ScatoPersistenceParticipant());
            var recorrido = srvRepositorio.ObtenerRecorrido(recorridoId);

            host.InArguments.InstanceId = recorrido.InstanciaWorkflow;
            host.InArguments.PesoNeto = recorrido.PesoBruto - recorrido.PesoTara;
            host.InArguments.NumeroDocumento = recorrido.NumeroDocumentoIngresoLegal;
            host.InArguments.FechaIngreso = recorrido.FechaInicio;
            host.InArguments.Km = null;
            host.InArguments.LocalidadId = null;
            host.InArguments.CentroId = recorrido.Centro.Id;
            host.InArguments.ClienteCodigoSap = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? "" : recorrido.Centro.CodigoSAP;
            host.InArguments.MaterialId = recorrido.Material.Id;
            host.InArguments.Patente = recorrido.Patente;
            host.InArguments.ProvinciaId = null;
            host.InArguments.TipoMovimiento = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? "SAL" : "ENT";
            host.InArguments.TransportistaId = recorrido.Transportista.Id;
            host.InArguments.InstanceId = recorrido.InstanciaWorkflow;

            var retorno = host.TestActivity();
            var request = retorno.First(f => f.Key == "Request").Value;
            var resultado = retorno.First(f => f.Key == "Resultado").Value;

            var transmision = conversor.Convertir<IngresosEgresosFazones, Dominio.Entidades.IngresosEgresosFazonesTransmisionASap>(((IngresosEgresosFazonesRequest)request).IngresosEgresosFazones);

            transmision.FuncionSap = FuncionSAP.IngresosEgresosFazones;
            transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
            transmision.Fecha = DateTime.Now;
            transmision.MensajeError = "";
            transmision.Estado = EstadoTransmisionASap.Error;

            return transmision;
        }

        private static readonly Regex Numeric = new Regex(@"^\d+$");

        public string Mov305(RecorridoDto recorrido)
        {
            try
            {
                var t = ObtenerLlegadaADestinoenRedespacho(recorrido.Id, servicio, servicioComandos);
                servicioComandos.Ejecutar(new ActualizarLlegadaADestinoEnRedespachosTransmisionASap() { Dto = t });
                log.Info("Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente");
                return "Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente";
            }
            catch (Exception e)
            {
                log.Error("Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso + ". " + e.Message);
                return "Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso;
            }
        }
        private LlegadaAdestinosEnRedespachosTransmisionASap ObtenerLlegadaADestinoenRedespacho(int recorridoId, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {

            var target = new LlegadaADestinoEnRedespachosGenerarRequest();
            var host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio);
            host.Extensions.Add(() => new ScatoPersistenceParticipant());
            var recorrido = srvRepositorio.ObtenerRecorrido(recorridoId);

            host.InArguments.FechaContab = recorrido.FechaEgreso;
            host.InArguments.Ejercicio = recorrido.FechaEgreso.Value.Year;
            host.InArguments.Documento = recorrido.NumeroDocumentoIngreso;
            host.InArguments.DocumentoInternoSap = "";

            var retorno = host.TestActivity();
            var request = retorno.First(f => f.Key == "Request").Value;
            var resultado = retorno.First(f => f.Key == "Resultado").Value;

            var transmision = conversor.Convertir<Mov305, LlegadaAdestinosEnRedespachosTransmisionASap>(((Mov305Request)request).Mov305);

            transmision.FuncionSap = FuncionSAP.LlegadaADestinosEnRedespachos;
            transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
            transmision.Fecha = DateTime.Now;
            transmision.MensajeError = "";
            transmision.Estado = EstadoTransmisionASap.Error;

            return transmision;
        }


        public string AjustedeDiferencias(RecorridoDto recorrido)
        {
            try
            {
                var t = ObtenerTransaccion(recorrido, servicio);
                servicioComandos.Ejecutar(new ActualizarAjusteDeDiferenciasEnRedespachosTransmisionASap { Dto = t });
                log.Info("Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente");
                return "Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente";
            }
            catch (Exception e)
            {
                log.Error("Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso + ". " + e.Message);
                return "Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso;
            }
        }

        private AjusteDeDiferenciasEnRedespachosTransmisionASap ObtenerTransaccion(RecorridoDto recorrido, IServicioRepositorio servicio)
        {
            var target = new MovimientoStockSapGenerarRequest();
            var host = WorkflowInvokerTest.Create(target);

            var orden = servicio.ObtenerCartaPorte(recorrido.Vehiculo.CartaPorteId);

            host.Extensions.Add(servicio);
            host.Extensions.Add(() => new ScatoPersistenceParticipant());

            host.InArguments.InstanceId = recorrido.InstanciaWorkflow;
            host.InArguments.CentroId = recorrido.Centro.Id;
            host.InArguments.MaterialId = recorrido.Material.Id;
            host.InArguments.Patente = recorrido.Patente;
            host.InArguments.Cantidad = recorrido.PesoBruto - recorrido.PesoTara;
            host.InArguments.CentroCosteId = orden.DestinoId;
            host.InArguments.ClaseExp = "CA";
            host.InArguments.FechaContab = recorrido.FechaEgreso ?? recorrido.FechaInicio;
            host.InArguments.FechaDoc = recorrido.FechaEgreso ?? recorrido.FechaInicio;
            host.InArguments.NroDocumento = recorrido.NumeroDocumentoIngreso;

            var retorno = host.TestActivity();
            var request = retorno.First(f => f.Key == "Request").Value;

            var transmision = conversor.Convertir<MovAjuste, AjusteDeDiferenciasEnRedespachosTransmisionASap>(((MovAjuste)request));

            transmision.FuncionSap = FuncionSAP.AjusteDeDiferencias;
            transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
            transmision.Fecha = DateTime.Now;
            transmision.MensajeError = "";
            transmision.Estado = EstadoTransmisionASap.Error;

            return transmision;
        }


        public string EgresosNoProductivos(RecorridoDto recorrido)
        {
            try
            {
                var t = ObtenerEgresosMaterialNoProductivo(recorrido.Id, servicio, servicioComandos);
                servicioComandos.Ejecutar(new ActualizarEgresosMaterialNoProductivoTransmisionASap() { Dto = t });
                log.Info("Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente");
                return "Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente";
            }
            catch (Exception e)
            {
                log.Error("Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso + ". " + e.Message);
                return "Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso;
            }
        }


        private EgresosNoProductivosTransmisionASap ObtenerEgresosMaterialNoProductivo(int recorridoId, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {
            srvRepositorio = new ChannelFactory<IServicioRepositorio>("ServicioRepositorio").CreateChannel();

            var target = new EgresosMaterialNoProductivoGenerarRequest();
            var host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio);
            host.Extensions.Add(() => new ScatoPersistenceParticipant());
            var recorrido = srvRepositorio.ObtenerRecorrido(recorridoId);
            var orden = srvRepositorio.ObtenerOrdenCargaInternaPorInstanceId(recorrido.InstanciaWorkflow);

            host.InArguments.CentroId = recorrido.Centro.Id;
            host.InArguments.TransportistaId = orden.TransportistaId;
            host.InArguments.ChoferId = orden.Chofer.Id;
            host.InArguments.Patente = orden.PatenteCamion;
            host.InArguments.PatenteAcoplado = orden.PatenteAcoplado;
            host.InArguments.MaterialId = orden.MaterialId;
            host.InArguments.InstanceId = recorrido.InstanciaWorkflow;
            host.InArguments.ClienteId = orden.DestinoId;
            host.InArguments.PesoNeto = recorrido.PesoBruto - recorrido.PesoTara;
            host.InArguments.Fecha = orden.FechaCreacion;

            var retorno = host.TestActivity();
            var request = retorno.First(f => f.Key == "Request").Value;
            var resultado = retorno.First(f => f.Key == "Resultado").Value;

            var transmision = conversor.Convertir<EgresosNoProductivos, EgresosNoProductivosTransmisionASap>(((EgresosNoProductivosRequest)request).EgresosNoProductivos);

            transmision.FuncionSap = FuncionSAP.EgresosMaterialNoProductivo;
            transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
            transmision.Fecha = DateTime.Now;
            transmision.MensajeError = "";
            transmision.Estado = EstadoTransmisionASap.Error;

            return transmision;
        }


        public string MOV975(RecorridoDto recorrido)
        {
            try
            {
                var t = ObtenerSalidaADestinoenRedespacho(recorrido, servicio, servicioComandos);
                servicioComandos.Ejecutar(new ActualizarSalidaDeOrigenEnRedespachosTransmisionASap() { Dto = t });
                log.Info("Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente");
                return "Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente";
            }
            catch (Exception e)
            {
                log.Error("Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso + ". " + e.Message);
                return "Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso;
            }
        }

        private Molinos.Scato.Dominio.Entidades.SalidaDeOrigenEnRedespachosTransmisionASap ObtenerSalidaADestinoenRedespacho(RecorridoDto recorrido, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {

            var target = new SalidaDeOrigenEnRedespachosGenerarRequest();

            var host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio);
            host.Extensions.Add(() => new ScatoPersistenceParticipant());

            host.InArguments.InstanceId = recorrido.InstanciaWorkflow;
            host.InArguments.Cantidad = recorrido.PesoBruto - recorrido.PesoTara;
            host.InArguments.CentroEmisorId = recorrido.Centro.Id;
            host.InArguments.ClaseExp = "CA";
            host.InArguments.FechaContab = DateTime.Now;
            host.InArguments.MaterialId = recorrido.Material.Id;
            host.InArguments.Precinto1Id = 0;
            host.InArguments.Precinto2Id = 0;
            //host.InArguments.Lote = "";
            host.InArguments.TipoDoc = recorrido.TipoDocumentoIngreso;
            host.InArguments.TransportistaId = recorrido.Transportista.Id;

            if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte)
            {
                var orden = srvRepositorio.ObtenerCartaPorte(recorrido.Vehiculo.CartaPorteId);
                host.InArguments.CentroReceptorId = orden.DestinoId;
                host.InArguments.ChoferId = orden.Chofer.Id;
                host.InArguments.FechaDoc = orden.FechaCP;
                host.InArguments.Kilometros = Convert.ToInt32(orden.KmRecorrer);
                host.InArguments.NroDocumento = orden.NroCartaPorte;
                host.InArguments.Patente = recorrido.Vehiculo.Patente;
                host.InArguments.PatenteAcoplado = recorrido.Vehiculo.PatenteAcoplado;


            }
            else if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenEntrePlantas)
            {
                var orden = srvRepositorio.ObtenerOrdenEntrePlantasPorInstanceId(recorrido.InstanciaWorkflow);
                host.InArguments.CentroReceptorId = orden.CentroDestinoId;
                host.InArguments.ChoferId = orden.Chofer.Id;
                host.InArguments.FechaDoc = orden.Fecha;
                host.InArguments.Kilometros = Convert.ToInt32(orden.KmRecorrer);
                host.InArguments.Patente = orden.PatenteCamion;
                host.InArguments.PatenteAcoplado = orden.PatenteAcoplado;
            }




            var retorno = host.TestActivity();
            var request = retorno.First(f => f.Key == "Request").Value;
            var resultado = retorno.First(f => f.Key == "Resultado").Value;

            var transmision = conversor.Convertir<Mov975, Molinos.Scato.Dominio.Entidades.SalidaDeOrigenEnRedespachosTransmisionASap>(((Mov975Request)request).Mov975);

            transmision.FuncionSap = FuncionSAP.SalidaDeOrigenEnRedespachos;
            transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
            transmision.Fecha = DateTime.Now;
            transmision.MensajeError = "";
            transmision.Estado = EstadoTransmisionASap.Error;

            return transmision;
        }


        public string InformarCupo(RecorridoDto recorrido)
        {
            try
            {
                var request = ObtenerRequestInformarCupo(recorrido.Id, servicio, servicioComandos);
                log.Info("request:" + (request != null).ToString());
                log.Info("Z_SDMF_Z2200NRequest:" + (request.Z_SDMF_Z2200N != null).ToString());
                var transmision = conversor.Convertir<Z_SDMF_Z2200N, Molinos.Scato.Dominio.Entidades.InformarCupoTransmisionASap>(request.Z_SDMF_Z2200N);
                log.Info("guardando en la base");
                transmision.FuncionSap = FuncionSAP.InformarCupo;
                transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
                transmision.Fecha = DateTime.Now;
                transmision.MensajeError = "";
                transmision.Estado = EstadoTransmisionASap.Error;

                servicioComandos.Ejecutar(new ActualizarInformarCupoTransmisionASap { Dto = transmision });
                log.Info("Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente");
                return "Carta de porte: " + recorrido.NumeroDocumentoIngreso + " procesada correctamente";
            }
            catch (Exception e)
            {
                log.Error("Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso + ". " + e.Message);
                return "Ocurrió un error procesando " + recorrido.NumeroDocumentoIngreso;
            }
        }

        private Z_SDMF_Z2200NRequest ObtenerRequestInformarCupo(int recorridoId, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {
            log.Info("ObtenerRequest");
            var resultado = new Resultado();
            Z_SDMF_Z2200NRequest request = null;
            try
            {
                var recorrido = srvRepositorio.ObtenerRecorrido(recorridoId);
                var muestraEnvioACamara = srvRepositorio.ObtenerUltimaMuestraEnvioACamaraPorCaladoId(recorrido.Calado.Id);
                ConfigurationManager.AppSettings["SepararAlmacenSustentable"] = "false";
                var guid = recorrido.InstanciaWorkflow;

                var cartaPorte = srvRepositorio.ObtenerCartaPortePorInstanceId(guid);
                var cupo = cartaPorte.Cupo;
                var fechaIngreso = recorrido.FechaInicio;
                var centroId = recorrido.Centro.Id;
                var fechaPesoTara = recorrido.PesoTaraFecha;
                var fechaEgreso = recorrido.FechaEgreso;
                var camionRechazado = recorrido.Rechazado;
                var instanceId = recorrido.InstanciaWorkflow;


                log.Info("Iniciando generación");
                var target = new InformarCupoGenerarRequest();
                var host = WorkflowInvokerTest.Create(target);
                host.Extensions.Add(srvRepositorio);
                host.Extensions.Add(servicioComandos
                    );
                host.Extensions.Add(() => new ScatoPersistenceParticipant());

                host.InArguments.CartaPorte = cartaPorte;
                host.InArguments.CodigoCupo = cupo;
                host.InArguments.FechaIngreso = fechaIngreso;
                host.InArguments.CentroId = centroId;
                host.InArguments.FechaPesadaTara = fechaPesoTara;
                host.InArguments.FechaEgreso = fechaEgreso;
                host.InArguments.CamionRechazado = camionRechazado;
                host.InArguments.InstanceId = instanceId;


                var retorno = host.TestActivity();
                request = (Z_SDMF_Z2200NRequest)retorno.First(f => f.Key == "Request").Value;
                var errores = (Resultado)retorno.First(f => f.Key == "Resultado").Value;
                foreach (var error in errores.Errores)
                {
                    log.Error(error.Key + " - " + error.Value);
                }
                log.Info("Request Generada\n");
            }
            catch (Exception e)
            {

                log.Info("ERRPR" + e.Message);
                log.Info("ERRPR" + e.StackTrace);
                resultado.Errores.Add("", e.Message);
            }
            log.Info("request" + (request != null).ToString());
            return request;
        }


    }
}
