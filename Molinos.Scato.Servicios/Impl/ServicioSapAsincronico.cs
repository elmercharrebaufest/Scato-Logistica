using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Modules;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioSapAsincronico : IServicioSapAsincronico
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly IConversor conversor;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly WaybillManagementPODv2 servicioMonsanto;

        public ServicioSapAsincronico(IServicioComandos servicioComandos, IServicioRepositorio servicio, IConversor conversor, ILogger log, ZSDWS_SCATO servicioSap, WaybillManagementPODv2 servicioMonsanto)
        {
            this.servicioComandos = servicioComandos;
            this.servicio = servicio;
            this.servicioSap = servicioSap;
            this.servicioMonsanto = servicioMonsanto;
            this.log = log;
            this.conversor = conversor;
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void IngresoPorCompraDeGranos(Guid idInstancia, Fill_Z1000 registro)
        {
            LoguearInicioEjecucion(idInstancia, "Z_MPZ1000");
            var transmision = conversor.Convertir<Fill_Z1000, IngresosPorCompraDeGranosTransmisionASap>(registro);
            foreach (var recepcion in registro.RecepcionesYDespachosII.Select(recepcionYRedespacho => conversor.Convertir<ZMPES0020, RecepcionYRedespacho>(recepcionYRedespacho)))
            {
                recepcion.IngresosPorCompraDeGranosTransmisionASap = transmision;
                transmision.RecepcionesYDespachosII.Add(recepcion);
            }
            transmision.FuncionSap = FuncionSAP.IngresosPorCompraDeGranos;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad IngresosPorCompraDeGranosTransmisionASap");
            try
            {
                var request = new Fill_Z1000Request
                {
                    Fill_Z1000 = registro
                };
                log.Info("Se Va a enviar la request Fill_Z1000");
                var respuesta = servicioSap.Fill_Z1000(request);
                if (respuesta.Fill_Z1000Response.Resultado.Any(res => res.MSGNR != "000"))
                {
                    log.Warn("Respuesta con errores en la ejecución de la función Z_MPZ1000");
                    transmision.MensajeError =
                        respuesta.Fill_Z1000Response.Resultado.First(res => res.MSGNR != "000").TEXT;
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error: {0}", transmision.MensajeError);
                }
                else
                {
                    log.Info("Respuesta Correcta en la ejecución de la función Z_MPZ1000");
                    transmision.MensajeError = "";
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarIngresosPorCompraDeGranosTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función Z_MPZ1000");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarIngresosPorCompraDeGranosTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void ZE7550(Guid idInstancia, Z_SDMF_RFC_ZE7550 registro)
        {
            LoguearInicioEjecucion(idInstancia, "ZE7550");
            var transmision = conversor.Convertir<Z_SDMF_RFC_ZE7550, ZE7550TransmisionASap>(registro);
            try
            {
                transmision.FuncionSap = FuncionSAP.ZE7550;
                transmision.InstanciaWorkflow = idInstancia;
                transmision.Fecha = DateTime.Now;
                transmision.Estado = EstadoTransmisionASap.Pendiente;
                log.Info("Se Creó la entidad ZE7550TransmisionASap");
                servicioComandos.Ejecutar(new ActualizarZE7550TransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función ZE7550");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarZE7550TransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void SalidaDeOrigenEnRedespachos(Guid idInstancia, Mov975 mov975)
        {
            LoguearInicioEjecucion(idInstancia, "ZSDFC_MOV975");
            var transmision = conversor.Convertir<Mov975, SalidaDeOrigenEnRedespachosTransmisionASap>(mov975);
            transmision.FuncionSap = FuncionSAP.SalidaDeOrigenEnRedespachos;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad SalidaDeOrigenEnRedespachosTransmisionASap");
            try
            {
                var request = new Mov975Request
                {
                    Mov975 = mov975
                };
                log.Info("Se Va a enviar la request Mov975");
                var respuesta = servicioSap.Mov975(request);

                if (respuesta.Mov975Response != null && !string.IsNullOrEmpty(respuesta.Mov975Response.Resultado.MBLNR))
                {
                    log.Info("Respuesta Correcta en la ejecución de la función Mov975");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                    {
                        InstanceId = idInstancia,
                        DocumentoInternoSap = respuesta.Mov975Response.Resultado.MBLNR,
                        NumeroDeDocumentoSap = respuesta.Mov975Response.Resultado.XBLNR
                    });
                }
                else if (respuesta.Mov975Response != null)
                {
                    log.Warn("Respuesta con errores en la ejecución de la función Mov975");
                    transmision.MensajeError = respuesta.Mov975Response.Resultado.TEXT;
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error: {0}", transmision.MensajeError);
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarSalidaDeOrigenEnRedespachosTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función ZSDFC_MOV975");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarSalidaDeOrigenEnRedespachosTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LlegadaADestinoEnRedespachos(Guid idInstancia, Mov305 mov)
        {
            LoguearInicioEjecucion(idInstancia, "ZSDFC_MOV305");
            var transmision = conversor.Convertir<Mov305, LlegadaAdestinosEnRedespachosTransmisionASap>(mov);
            transmision.FuncionSap = FuncionSAP.LlegadaADestinosEnRedespachos;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad LlegadaAdestinosEnRedespachosTransmisionASap");
            try
            {
                var request = new Mov305Request
                {
                    Mov305 = mov
                };
                log.Info("Se Va a enviar la request Mov305");
                var respuesta = servicioSap.Mov305(request);

                if (respuesta.Mov305Response != null && !string.IsNullOrEmpty(respuesta.Mov305Response.Resultado.MBLNR))
                {
                    log.Info("Respuesta Correcta en la ejecución de la función Mov305");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                }
                else if (respuesta.Mov305Response != null)
                {
                    log.Warn("Respuesta con errores en la ejecución de la función Mov305");
                    transmision.MensajeError = respuesta.Mov305Response.Resultado.TEXT;
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error: {0}", transmision.MensajeError);
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarLlegadaADestinoEnRedespachosTransmisionASap { Dto = transmision });

            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función Mov305");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarLlegadaADestinoEnRedespachosTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void EgresosMaterialNoProductivo(Guid idInstancia, EgresosNoProductivos egresosNoProductivos)
        {
            LoguearInicioEjecucion(idInstancia, "ZSDFC_EgresosNoProductivos");
            var transmision = conversor.Convertir<EgresosNoProductivos, EgresosNoProductivosTransmisionASap>(egresosNoProductivos);
            transmision.FuncionSap = FuncionSAP.EgresosMaterialNoProductivo;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;

            log.Info("Se Creó la entidad EgresosNoProductivosTransmisionASap");
            try
            {
                var request = new EgresosNoProductivosRequest
                {
                    EgresosNoProductivos = egresosNoProductivos
                };
                log.Info("Se Va a enviar la request EgresosNoProductivos");

                var respuesta = servicioSap.EgresosNoProductivos(request);
                if (respuesta.EgresosNoProductivosResponse != null && !string.IsNullOrEmpty(respuesta.EgresosNoProductivosResponse.Resultado.MBLNR))
                {
                    log.Info("Respuesta Correcta en la ejecución de la función EgresosNoProductivos");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                    {
                        InstanceId = idInstancia,
                        DocumentoInternoSap = respuesta.EgresosNoProductivosResponse.Resultado.MBLNR,
                        NumeroDeDocumentoSap = respuesta.EgresosNoProductivosResponse.Resultado.XBLNR
                    });
                }
                else if (respuesta.EgresosNoProductivosResponse != null)
                {
                    log.Warn("Respuesta con errores en la ejecución de la función EgresosNoProductivos");
                    transmision.MensajeError = respuesta.EgresosNoProductivosResponse.Resultado.TEXT;
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error: {0}", transmision.MensajeError);
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarEgresosMaterialNoProductivoTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función ZSDFC_EgresosMaterialNoProductivo");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarEgresosMaterialNoProductivoTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void IngresosEgresosFazones(Guid idInstancia, IngresosEgresosFazones ingresosEgresosFazones)
        {
            LoguearInicioEjecucion(idInstancia, "ZSDFC_IngresosEgresosFazones");
            var transmision = conversor.Convertir<IngresosEgresosFazones, IngresosEgresosFazonesTransmisionASap>(ingresosEgresosFazones);
            transmision.FuncionSap = FuncionSAP.IngresosEgresosFazones;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;

            log.Info("Se Creó la entidad IngresosEgresosFazonesTransmisionASap");
            try
            {
                var request = new IngresosEgresosFazonesRequest
                {
                    IngresosEgresosFazones = ingresosEgresosFazones
                };
                log.Info("Se Va a enviar la request IngresosEgresosFazones");
                var respuesta = servicioSap.IngresosEgresosFazones(request);

                if (respuesta.IngresosEgresosFazonesResponse != null && !string.IsNullOrEmpty(respuesta.IngresosEgresosFazonesResponse.Resultado.MBLNR))
                {
                    log.Info("Respuesta correcta en la ejecución de la función IngresosEgresosFazones");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                    {
                        InstanceId = idInstancia,
                        DocumentoInternoSap = respuesta.IngresosEgresosFazonesResponse.Resultado.MBLNR,
                        NumeroDeDocumentoSap = respuesta.IngresosEgresosFazonesResponse.Resultado.XBLNR
                    });
                }
                else if (respuesta.IngresosEgresosFazonesResponse != null)
                {
                    log.Warn("Respuesta con errores en la ejecución de la función IngresosEgresosFazones");
                    transmision.MensajeError = respuesta.IngresosEgresosFazonesResponse.Resultado.TEXT;
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error: {0}", transmision.MensajeError);

                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarIngresosEgresosFazonesTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función ZSDFC_IngresosEgresosFazones");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarIngresosEgresosFazonesTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void PesaNeto(Guid idInstancia, PesaNeto pesaNeto)
        {
            LoguearInicioEjecucion(idInstancia, "Z_SDMF_RFC_PESANETO");
            var transmision = conversor.Convertir<PesaNeto, PesaNetoTransmisionASap>(pesaNeto);
            transmision.FuncionSap = FuncionSAP.PesaNeto;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;

            log.Info("Se Creó la entidad PesaNetoTransmisionASap");
            try
            {
                var request = new PesaNetoRequest
                {
                    PesaNeto = pesaNeto
                };
                log.Info("Se Va a enviar la request PesaNeto");

                var respuesta = servicioSap.PesaNeto(request);
                if (respuesta.PesaNetoResponse != null && respuesta.PesaNetoResponse.Mensajes.Any() && respuesta.PesaNetoResponse.Mensajes.All(x => x.MBLNR == "000"))
                {
                    log.Info("Respuesta correcta en la ejecución de la función PesaNeto");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                    {
                        InstanceId = idInstancia,
                        DocumentoInternoSap = respuesta.PesaNetoResponse.Mensajes.FirstOrDefault().MBLNR,
                        NumeroDeDocumentoSap = respuesta.PesaNetoResponse.Mensajes.FirstOrDefault().XBLNR
                    });
                }
                else
                {
                    log.Warn("Respuesta con errores en la ejecución de la función PesaNeto");
                    transmision.MensajeError = respuesta.PesaNetoResponse.Mensajes.FirstOrDefault() != null ? respuesta.PesaNetoResponse.Mensajes.FirstOrDefault().TEXT : "Respuesta vacía";
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error: {0}", transmision.MensajeError);

                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarPesaNetoTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función PesaNeto");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarPesaNetoTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AjusteDeDiferenciasDePesoEnRedespachos(Guid idInstancia, MovAjuste movAjuste)
        {
            LoguearInicioEjecucion(idInstancia, "ZSDFC_MOVAJUSTE");
            var transmision = conversor.Convertir<MovAjuste, AjusteDeDiferenciasEnRedespachosTransmisionASap>(movAjuste);
            transmision.FuncionSap = FuncionSAP.AjusteDeDiferencias;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad AjusteDeDiferenciasEnRedespachosTransmisionASap");
            try
            {
                var request = new MovAjusteRequest { MovAjuste = movAjuste };
                log.Info("Se Va a enviar la request MovAjuste");
                var respuesta = servicioSap.MovAjuste(request);

                if (respuesta.MovAjusteResponse != null && respuesta.MovAjusteResponse.Resultado.MSGNR == "000")
                {
                    log.Info("Respuesta correcta en la ejecución de la función MovAjuste");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                }
                else if (respuesta.MovAjusteResponse != null)
                {
                    log.Warn("Respuesta con errores en la ejecución de la función MovAjuste");
                    transmision.MensajeError = respuesta.MovAjusteResponse.Resultado.TEXT;
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error :" + transmision.MensajeError);
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarAjusteDeDiferenciasEnRedespachosTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función MovAjuste");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarAjusteDeDiferenciasEnRedespachosTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void FletesDobleTramo(Guid idInstancia, FletesDobleTramo fletesDobleTramo)
        {
            LoguearInicioEjecucion(idInstancia, "Z_SDMF_Z4030");
            var transmision = conversor.Convertir<FletesDobleTramo, FletesDobleTramoTransmisionASap>(fletesDobleTramo);
            transmision.FuncionSap = FuncionSAP.FletesDobleTramo;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad FletesDobleTramoTransmisionASap");
            try
            {
                var request = new FletesDobleTramoRequest
                {
                    FletesDobleTramo = fletesDobleTramo
                };
                log.Info("Se Va a enviar la request FletesDobleTramo");
                var respuesta = servicioSap.FletesDobleTramo(request);

                if (respuesta.FletesDobleTramoResponse != null && !string.IsNullOrEmpty(respuesta.FletesDobleTramoResponse.Resultado.MBLNR))
                {
                    log.Info("Respuesta Correcta en la ejecución de la función FletesDobleTramo");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                }
                else if (respuesta.FletesDobleTramoResponse != null)
                {
                    log.Warn("Respuesta con errores en la ejecución de la función FletesDobleTramo");
                    transmision.MensajeError = respuesta.FletesDobleTramoResponse.Resultado.TEXT;
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error: {0}", transmision.MensajeError);
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarFletesDobleTramoTransmisionASap { Dto = transmision });

            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función FletesDobleTramo");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarFletesDobleTramoTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void InformarCupo(Guid idInstancia, Z_SDMF_Z2200N informarCupo)
        {
            LoguearInicioEjecucion(idInstancia, "INFORMARCUPO");
            var transmision = conversor.Convertir<Z_SDMF_Z2200N, InformarCupoTransmisionASap>(informarCupo);
            transmision.FuncionSap = FuncionSAP.InformarCupo;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad InformarCupoTransmisionASap");
            try
            {
                var request = new Z_SDMF_Z2200NRequest
                {
                    Z_SDMF_Z2200N = informarCupo
                };
                log.Info("Se Va a enviar la request InformarCupo");
                var respuesta = servicioSap.Z_SDMF_Z2200N(request);


                if (respuesta.Z_SDMF_Z2200NResponse != null &&
                   respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO.Any() &&
                   respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO.All(x => x.MSGNR == "000"))
                {
                    log.Info("Respuesta correcta en la ejecución de la función InformarCupo");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                }
                else if (respuesta.Z_SDMF_Z2200NResponse != null)
                {
                    log.Warn("Respuesta con errores en la ejecución de la función InformarCupo");
                    transmision.MensajeError = respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO.FirstOrDefault() != null ?
                        respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO.First(x => x.MSGNR != "000").TEXT : "Respuesta Vacia";
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error :" + transmision.MensajeError);
                }

                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarInformarCupoTransmisionASap { Dto = transmision });

            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función InformarCupo");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarInformarCupoTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void EgresoSinFleteFazones(Guid idInstancia, EgresoSinFleteFazones mov)
        {
            LoguearInicioEjecucion(idInstancia, "ZSDFC_MOV291");
            var transmision = conversor.Convertir<EgresoSinFleteFazones, EgresoSinFleteFasonesTransmisionASap>(mov);
            transmision.FuncionSap = FuncionSAP.EgresoSinFleteFazones;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad EgresoSinFleteFasonesTransmisionASap");
            try
            {
                var request = new EgresoSinFleteFazonesRequest()
                {
                    EgresoSinFleteFazones = mov
                };
                log.Info("Se Va a enviar la request Mov291");
                var respuesta = servicioSap.EgresoSinFleteFazones(request);

                if (respuesta.EgresoSinFleteFazonesResponse != null && !string.IsNullOrEmpty(respuesta.EgresoSinFleteFazonesResponse.Resultado.MBLNR))
                {
                    log.Info("Respuesta Correcta en la ejecución de la función Mov291");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                }
                else if (respuesta.EgresoSinFleteFazonesResponse != null)
                {
                    log.Warn("Respuesta con errores en la ejecución de la función Mov291");
                    transmision.MensajeError = respuesta.EgresoSinFleteFazonesResponse.Resultado.TEXT;
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error: {0}", transmision.MensajeError);
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarEgresoSinFleteFasonesTransmisionASap() { Dto = transmision });

            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función Mov291");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarEgresoSinFleteFasonesTransmisionASap { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RegistrarCartaDePorteTransporteAutomotor(Guid idInstancia, CartaPorteTransporteAutomotorRegistro cartaPorteRegistro)
        {
            LoguearInicioEjecucion(idInstancia, "RegistrarCartaDePorteTransporteAutomotor");
            var transmision = conversor.Convertir<CartaPorteTransporteAutomotorRegistro, CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto>(cartaPorteRegistro);
            transmision.FuncionSap = FuncionSAP.CartaPorteTransporteAutomotorRegistro;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad CartaPorteRegistroTransmisionAMonsanto");
            try
            {
                var request = new registrarCartaDePorte(
                    new ParametrosRegistro
                    {
                        Item = cartaPorteRegistro
                    }
                );
                log.Info("Se Va a enviar la request ParametrosRegistro");
                var respuesta = servicioMonsanto.registrarCartaDePorte(request);
                log.Info("Respuesta Correcta en la ejecución de la función registrarCartaDePorte");
                transmision.Estado = EstadoTransmisionASap.Correcto;
                transmision.MensajeError = "";
                var muestra = respuesta.respuesta.Item as MuestraRequerida;
                if (muestra != null)
                {
                    servicioComandos.Ejecutar(new ModificarCartaDePorteRegistradaServicioMonsanto
                    {
                        InstanceId = transmision.InstanciaWorkflow,
                        TipoAnalisis = muestra.tipoAnalisis.ToString(),
                        LaboratorioRazonSocial = muestra.laboratorio == null ? string.Empty : muestra.laboratorio.razonSocial,
                        LaboratorioCuit = muestra.laboratorio == null ? string.Empty : muestra.laboratorio.cuit.ToString(CultureInfo.InvariantCulture),
                    });
                }
                log.Debug("Correcto " + FuncionSAP.CartaPorteTransporteAutomotorRegistro);
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarCartaPorteTransporteAutomotorRegistroTransmisionAMonsanto() { Dto = transmision });

            }
            catch (System.ServiceModel.FaultException<webServiceExceptionV2FaultDetailsBean> e)
            {
                log.Warn(e, "Error en la ejecución de la función registrarCartaDePorte");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Detail.errores != null && e.Detail.errores.Any() ? e.Detail.errores.First().descripcion.Truncate(45) + "(" + e.Detail.errores.First().codigo + ")" : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarCartaPorteTransporteAutomotorRegistroTransmisionAMonsanto { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función registrarCartaDePorte");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarCartaPorteTransporteAutomotorRegistroTransmisionAMonsanto { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RegistrarCartaDePorteVagonFerroviario(Guid idInstancia, CartaPorteVagonFerroviarioRegistro cartaPorteRegistro)
        {
            LoguearInicioEjecucion(idInstancia, "RegistrarCartaDePorteVagonFerroviario");
            var transmision = conversor.Convertir<CartaPorteVagonFerroviarioRegistro, CartaPorteVagonFerroviarioRegistroTransmisionAMonsanto>(cartaPorteRegistro);
            transmision.FuncionSap = FuncionSAP.CartaPorteVagonFerroviarioRegistro;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad CartaPorteRegistroTransmisionAMonsanto");
            try
            {
                var request = new registrarCartaDePorte(
                    new ParametrosRegistro
                    {
                        Item = cartaPorteRegistro
                    }
                );
                log.Info("Se Va a enviar la request ParametrosRegistro");
                var respuesta = servicioMonsanto.registrarCartaDePorte(request);
                log.Info("Respuesta Correcta en la ejecución de la función registrarCartaDePorte");
                transmision.Estado = EstadoTransmisionASap.Correcto;
                transmision.MensajeError = "";
                var muestra = respuesta.respuesta.Item as MuestraRequerida;
                if (muestra != null)
                {
                    servicioComandos.Ejecutar(new ModificarCartaDePorteRegistradaServicioMonsanto
                    {
                        InstanceId = transmision.InstanciaWorkflow,
                        TipoAnalisis = muestra.tipoAnalisis.ToString(),
                        LaboratorioRazonSocial = muestra.laboratorio == null ? string.Empty : muestra.laboratorio.razonSocial,
                        LaboratorioCuit = muestra.laboratorio == null ? string.Empty : muestra.laboratorio.cuit.ToString(CultureInfo.InvariantCulture),
                    });
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarCartaPorteVagonFerroviarioRegistroTransmisionAMonsanto() { Dto = transmision });

            }
            catch (System.ServiceModel.FaultException<webServiceExceptionV2FaultDetailsBean> e)
            {
                log.Warn(e, "Error en la ejecución de la función registrarCartaDePorte");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Detail.errores != null && e.Detail.errores.Any() ? e.Detail.errores.First().descripcion.Truncate(45) + "(" + e.Detail.errores.First().codigo + ")" : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarCartaPorteVagonFerroviarioRegistroTransmisionAMonsanto { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función registrarCartaDePorte");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarCartaPorteVagonFerroviarioRegistroTransmisionAMonsanto { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RegistrarMuestreoYPesajeTransporteAutomotor(Guid idInstancia, MuestreoPesajeTransporteAutomotor muestreoRegistro)
        {
            LoguearInicioEjecucion(idInstancia, "RegistrarMuestreoYPesajeTransporteAutomotor");
            var transmision = conversor.Convertir<MuestreoPesajeTransporteAutomotor, MuestreoPesajeTransporteAutomotorTransmisionAMonsanto>(muestreoRegistro);
            transmision.FuncionSap = FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad MuestreoPesajeTransporteAutomotorTransmisionAMonsanto");
            try
            {
                var request = new registrarMuestreoYPesaje(new registerSampleAndWeightRequest
                {
                    Item = muestreoRegistro
                });
                log.Info("Se Va a enviar la request ParametrosRegistro");
                var respuesta = servicioMonsanto.registrarMuestreoYPesaje(request);

                if (!String.IsNullOrEmpty(respuesta.@return) && respuesta.@return == "OK")
                {
                    log.Info("Respuesta Correcta en la ejecución de la función RegistrarMuestreoYPesajeVagonFerroviario");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarMuestreoPesajeTransporteAutomotorTransmisionAMonsanto() { Dto = transmision });

            }
            catch (System.ServiceModel.FaultException<webServiceExceptionV2FaultDetailsBean> e)
            {
                log.Warn(e, "Error en la ejecución de la función registrarMuestreoYPesaje");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Detail.errores != null && e.Detail.errores.Any() ? e.Detail.errores.First().descripcion.Truncate(45) + "(" + e.Detail.errores.First().codigo + ")" : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarMuestreoPesajeTransporteAutomotorTransmisionAMonsanto { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función registrarMuestreoYPesaje");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarMuestreoPesajeTransporteAutomotorTransmisionAMonsanto { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RegistrarMuestreoYPesajeVagonFerroviario(Guid idInstancia, MuestreoPesajeVagonFerroviario muestreoRegistro)
        {
            LoguearInicioEjecucion(idInstancia, "RegistrarMuestreoYPesajeVagonFerroviario");
            var transmision = conversor.Convertir<MuestreoPesajeVagonFerroviario, MuestreoPesajeVagonFerroviarioTransmisionAMonsanto>(muestreoRegistro);
            foreach (var recepcion in muestreoRegistro.datosPorVagon.Select(x => conversor.Convertir<vagon, Vagon>(x)))
            {
                recepcion.MuestreoPesajeVagonFerroviarioTransmisionAMonsanto = transmision;
                transmision.DatosPorVagon.Add(recepcion);
            }
            transmision.FuncionSap = FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;
            log.Info("Se Creó la entidad MuestreoPesajeVagonFerroviarioTransmisionAMonsanto");
            try
            {
                var request = new registrarMuestreoYPesaje(
                    new registerSampleAndWeightRequest
                    {
                        Item = muestreoRegistro
                    }
                );
                log.Info("Se Va a enviar la request ParametrosRegistro");
                var respuesta = servicioMonsanto.registrarMuestreoYPesaje(request);

                if (!String.IsNullOrEmpty(respuesta.@return) && respuesta.@return == "OK")
                {
                    log.Info("Respuesta Correcta en la ejecución de la función RegistrarMuestreoYPesajeVagonFerroviario");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarMuestreoPesajeVagonFerroviarioTransmisionAMonsanto() { Dto = transmision });

            }
            catch (System.ServiceModel.FaultException<webServiceExceptionV2FaultDetailsBean> e)
            {
                log.Warn(e, "Error en la ejecución de la función registrarMuestreoYPesaje");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Detail.errores != null && e.Detail.errores.Any() ? e.Detail.errores.First().descripcion.Truncate(45) + "(" + e.Detail.errores.First().codigo + ")" : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarMuestreoPesajeVagonFerroviarioTransmisionAMonsanto { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función registrarMuestreoYPesaje");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a:  " + transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarMuestreoPesajeVagonFerroviarioTransmisionAMonsanto { Dto = transmision });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }


        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void IngresosBodega(Guid idInstancia, IngresosBodegaAsincronicoDto ingresosBodega)
        {
            LoguearInicioEjecucion(idInstancia, "IngresosBodega");
            var transmision = conversor.Convertir<IngresosBodega, IngresosBodegaTransmisionASap>(ingresosBodega.IngresosBodega.IngresosBodega);
            transmision.FuncionSap = FuncionSAP.IngresosBodega;
            transmision.InstanciaWorkflow = idInstancia;
            transmision.Fecha = DateTime.Now;
            transmision.Estado = EstadoTransmisionASap.Pendiente;

            log.Info("Se Creó la entidad IngresosBodegaTransmisionASap");
            try
            {

                var request = new IngresosBodegaRequest
                {
                    IngresosBodega = ingresosBodega.IngresosBodega.IngresosBodega
                };
                log.Info("Se Va a enviar la request IngresosBodega");

                var respuesta = servicioSap.IngresosBodega(request);
                if (respuesta.IngresosBodegaResponse != null && !string.IsNullOrEmpty(respuesta.IngresosBodegaResponse.EX_RESULTADO.MBLNR))
                {
                    log.Info("Respuesta Correcta en la ejecución de la función IngresosBodega");
                    transmision.Estado = EstadoTransmisionASap.Correcto;
                    transmision.MensajeError = "";
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                    {
                        InstanceId = idInstancia,
                        DocumentoInternoSap = respuesta.IngresosBodegaResponse.EX_RESULTADO.MBLNR,
                        NumeroDeDocumentoSap = respuesta.IngresosBodegaResponse.EX_RESULTADO.XBLNR
                    });
                }
                else if (respuesta.IngresosBodegaResponse != null)
                {
                    log.Warn("Respuesta con errores en la ejecución de la función IngresosBodega");
                    transmision.MensajeError = respuesta.IngresosBodegaResponse.EX_RESULTADO.TEXT;
                    transmision.Estado = EstadoTransmisionASap.Error;
                    log.Warn("Error: {0}", transmision.MensajeError);
                }
                log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                servicioComandos.Ejecutar(new ActualizarIngresosBodegaTransmisionASap { Dto = transmision, TipoBinId = ingresosBodega.TipoBinId });
            }
            catch (Exception e)
            {
                log.Warn(e, "Error en la ejecución de la función IngresosBodega");
                try
                {
                    transmision.Estado = EstadoTransmisionASap.Pendiente;
                    transmision.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    log.Info("Se actualiza el estado de la transmisión a: {0}", transmision.Estado);
                    servicioComandos.Ejecutar(new ActualizarIngresosBodegaTransmisionASap { Dto = transmision, TipoBinId = ingresosBodega.TipoBinId });
                }
                catch (Exception e2)
                {
                    log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    throw;
                }
                throw;
            }
        }


        private void LoguearInicioEjecucion(Guid idInstancia, string funcionSap)
        {
            if (log.IsDebugEnabled)
            {
                var messageProperty = ObtenerMessageProperty();
                if (messageProperty != null)
                {
                    log.Debug(
                        "Ejecutando servicio SAP {0} para la instancia {1}. Intento número {2}", funcionSap,
                        idInstancia, (messageProperty.MoveCount / 2) + 1);
                }
            }
        }

        private static MsmqMessageProperty ObtenerMessageProperty()
        {
            object valor;
            OperationContext.Current.IncomingMessageProperties.TryGetValue("MsmqMessageProperty", out valor);
            var messageProperty = valor as MsmqMessageProperty;
            return messageProperty;
        }
    }
}