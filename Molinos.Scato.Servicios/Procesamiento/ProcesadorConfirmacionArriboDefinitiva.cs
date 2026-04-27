using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Net;
using System.ServiceModel;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConfirmacionArriboDefinitiva : ProcesadorComando<ConfirmarArriboDefinitivo>
    {
        private const short TIPO_CPE_AUTOMOTOR = 74;
        private const short TIPO_CPE_FERROVIARIO = 75;
        private const short CODIGO_RAMAL_BELGRANO_DEFAULT = 5;
        private const string ESTADO_CONFIRMADO = "CN";
        private const string CONFIG_LOGUEAR_REQUESTS = "LoguearRequestsCtg";
        private const string CODIGO_ERROR_BAJA = "CodigoDeBaja";

        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;
        private readonly IServicioComandos servicioComandos;

        public ProcesadorConfirmacionArriboDefinitiva(
            IRepositorio repositorio,
            IConversor conversor,
            ILogger log,
            CpePortType serviceAfipCpe,
            IAccesoWsCtg accesoWsCtg,
            IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ConfirmarArriboDefinitivo comando)
        {
            ConfigurarSeguridadSsl();
            var resultado = new Resultado();

            try
            {
                Validar(comando, resultado);
                if (resultado.HayErrores)
                    return resultado;

                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                var recorrido = Repositorio.Obtener<Recorrido>(g => g.InstanciaWorkflow == comando.WorkflowId);
                var auth = ObtenerAutorizacion(centro, resultado);
                var tipoCpe = ObtenerTipoCpe(comando.Dto.TipoVehiculo);

                if (ValidarEstadoCpe(comando, tipoCpe, auth))
                {
                    Log.Debug("CPE {0} ya está confirmada definitivamente, no se requiere acción", comando.Dto.NroCartaPorte);
                    ActualizarBajaCTGDefinitiva(comando, resultado);
                    return resultado;
                }

                var request = EsTipoCpeAutomotor(tipoCpe)
                    ? ConstruirRequestConfirmacionAutomotor(comando, recorrido, auth)
                    : ConstruirRequestConfirmacionFerroviaria(comando, recorrido, auth);

                RegistrarRequest(comando, request);

                var response = EsTipoCpeAutomotor(tipoCpe)
                    ? EjecutarConfirmacionAutomotorEnAfip(request as confirmacionDefinitivaCPEAutomotorRequest)
                    : EjecutarConfirmacionFerroviariaEnAfip(request as confirmacionDefinitivaCPEFerroviariaRequest);

                ProcesarRespuestaAfip(response, resultado, request, comando, recorrido, centro, tipoCpe);
            }
            catch (FaultException ex)
            {
                Log.Error(ex, "Error de servicio AFIP al confirmar arribo definitivo de CTG: {0}", comando.Dto.NroCartaPorte);
                resultado.Errores.Add(CODIGO_ERROR_BAJA, "Error, el servicio de AFIP nos responde: " + ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al confirmar arribo definitivo de CTG: {0}", comando.Dto.NroCartaPorte);
                resultado.Errores.Add(CODIGO_ERROR_BAJA, Textos.Error_Generico);
            }
            finally
            {
                ActualizarBajaCTGDefinitiva(comando, resultado);
            }

            return resultado;
        }

        private void ConfigurarSeguridadSsl()
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        private void Validar(ConfirmarArriboDefinitivo comando, Resultado resultado)
        {
            var centro = Repositorio.Obtener<Centro>(comando.CentroId);
            if (centro == null)
            {
                resultado.Errores.Add(CODIGO_ERROR_BAJA, string.Format(Textos.Error_Requerido, Textos.Centro));
                return;
            }

            var recorrido = Repositorio.Obtener<Recorrido>(g => g.InstanciaWorkflow == comando.WorkflowId);
            if (recorrido == null || !recorrido.PesoBruto.HasValue || !recorrido.PesoTara.HasValue)
            {
                Log.Error("ProcesadorConfirmacionArriboDefinitivo - PESO NO ENCONTRADO");
                resultado.Errores.Add(CODIGO_ERROR_BAJA, "No se puede ejecutar la confirmacion definitiva de un camión sin peso");
            }
        }

        private Auth ObtenerAutorizacion(Centro centro, Resultado resultado)
        {
            Log.Debug("Obteniendo autorización AFIP para CUIT: {0}", centro.Cuit);
            var cuitRepresentado = LimpiarCuit(centro.Cuit);
            return accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);
        }

        private bool ValidarEstadoCpe(ConfirmarArriboDefinitivo comando, short tipoCpe, Auth auth)
        {
            var nroCTG = Convert.ToInt64(comando.Dto.NroCartaPorte);

            return EsTipoCpeAutomotor(tipoCpe)
                ? ConsultarEstadoCpeAutomotor(nroCTG, auth)
                : ConsultarEstadoCpeFerroviaria(nroCTG, auth);
        }

        private bool ConsultarEstadoCpeAutomotor(long nroCTG, Auth auth)
        {
            var consulta = serviceAfipCpe.consultarCPEAutomotor(new consultarCPEAutomotorRequest
            {
                auth = auth,
                solicitud = new ConsultarAutomotorSolicitud
                {
                    nroCTG = nroCTG,
                    nroCTGSpecified = true
                }
            });

            return consulta?.respuesta?.cabecera?.estado == ESTADO_CONFIRMADO;
        }

        private bool ConsultarEstadoCpeFerroviaria(long nroCTG, Auth auth)
        {
            var consulta = serviceAfipCpe.consultarCPEFerroviaria(new consultarCPEFerroviariaRequest
            {
                auth = auth,
                solicitud = new ConsultarFerroviariaSolicitud
                {
                    nroCTG = nroCTG,
                    nroCTGSpecified = true
                }
            });

            return consulta?.respuesta?.cabecera?.estado == ESTADO_CONFIRMADO;
        }

        private object ConstruirRequestConfirmacionAutomotor(
            ConfirmarArriboDefinitivo comando,
            Recorrido recorrido,
            Auth auth)
        {
            return new confirmacionDefinitivaCPEAutomotorRequest
            {
                auth = auth,
                solicitud = new ConfirmacionAutomotorSolicitud
                {
                    cuitSolicitante = long.Parse(LimpiarCuit(comando.Dto.TitularCartaPorteCuil)),
                    pesoBrutoDescarga = recorrido.PesoBruto ?? 0,
                    pesoTaraDescarga = recorrido.PesoTara ?? 0,
                    cartaPorte = new AfipCPDigitalService.CartaPorte
                    {
                        nroOrden = int.Parse(comando.Dto.CTG),
                        sucursal = comando.Dto.Sucursal ?? 0,
                        tipoCPE = TIPO_CPE_AUTOMOTOR
                    },
                    intervinientes = null
                }
            };
        }

        private object ConstruirRequestConfirmacionFerroviaria(
            ConfirmarArriboDefinitivo comando,
            Recorrido recorrido,
            Auth auth)
        {
            var codigoRamal = comando.Dto.CodigoRamalAfip.HasValue
                ? (short)comando.Dto.CodigoRamalAfip.Value
                : CODIGO_RAMAL_BELGRANO_DEFAULT;

            return new confirmacionDefinitivaCPEFerroviariaRequest
            {
                auth = auth,
                solicitud = new ConfirmacionFerroviariaSolicitud
                {
                    cuitSolicitante = long.Parse(LimpiarCuit(comando.Dto.TitularCartaPorteCuil)),
                    pesoBrutoDescarga = recorrido.PesoBruto ?? 0,
                    pesoTaraDescarga = recorrido.PesoTara ?? 0,
                    cartaPorte = new AfipCPDigitalService.CartaPorte
                    {
                        nroOrden = int.Parse(comando.Dto.CTG),
                        sucursal = comando.Dto.Sucursal ?? 0,
                        tipoCPE = TIPO_CPE_FERROVIARIO
                    },
                    ramalDescarga = new Ramal
                    {
                        codigo = codigoRamal
                    }
                }
            };
        }

        private object EjecutarConfirmacionAutomotorEnAfip(confirmacionDefinitivaCPEAutomotorRequest request)
        {
            Log.Debug("Ejecutando confirmación definitiva automotor en AFIP");
            return serviceAfipCpe.confirmacionDefinitivaCPEAutomotor(request);
        }

        private object EjecutarConfirmacionFerroviariaEnAfip(confirmacionDefinitivaCPEFerroviariaRequest request)
        {
            Log.Debug("Ejecutando confirmación definitiva ferroviaria en AFIP");
            return serviceAfipCpe.confirmacionDefinitivaCPEFerroviaria(request);
        }

        private void RegistrarRequest(ConfirmarArriboDefinitivo comando, object request)
        {
            try
            {
                if (ConfigurationManager.AppSettings[CONFIG_LOGUEAR_REQUESTS] == "1")
                {
                    Repositorio.Agregar(new ControlRecorrido
                    {
                        Actividad = nameof(ProcesadorConfirmacionArriboDefinitiva),
                        Fecha = DateTime.Now,
                        Comentario = request.ToXml(),
                        NombreUsuario = string.Empty,
                        WorkflowInstanceId = comando.WorkflowId,
                    });
                    Repositorio.GuardarCambios();
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error al registrar request AFIP CTG: {0}", ex.Message);
            }
        }

        private void ProcesarRespuestaAfip(
            object response,
            Resultado resultado,
            object request,
            ConfirmarArriboDefinitivo comando,
            Recorrido recorrido,
            Centro centro,
            short tipoCpe)
        {
            string estado = string.Empty;
            byte[] pdf = null;
            if (tipoCpe == TIPO_CPE_AUTOMOTOR)
            {
                var respuestaAutomotor = response as confirmacionDefinitivaCPEAutomotorResponse;
                estado = respuestaAutomotor?.respuesta?.cabecera?.estado;
                pdf = respuestaAutomotor?.respuesta?.pdf;
            }
            else
            {
                var respuestaFerroviaria = response as confirmacionDefinitivaCPEFerroviariaResponse;
                estado = respuestaFerroviaria?.respuesta?.cabecera?.estado;
                pdf = respuestaFerroviaria?.respuesta?.pdf;
            }

            if (estado == ESTADO_CONFIRMADO && !resultado.HayErrores)
            {
                GuardarPdfSiExiste(pdf, comando, recorrido, centro);
                RegistrarLogAfip("ConfirmarArriboDefinitivo", request.ToXml(), response.ToXml());
                Log.Debug("Confirmación definitiva automotor procesada correctamente para CTG: {0}", comando.Dto.NroCartaPorte);
            }
            else
            {
                Log.Error("Respuesta inválida de AFIP para CTG: {0}", comando.Dto.NroCartaPorte);
            }
        }

        private bool ValidarRespuestaExitosaAutomotor(confirmacionDefinitivaCPEAutomotorResponse response, Resultado resultado)
        {
            return response?.respuesta?.cabecera?.estado == ESTADO_CONFIRMADO && !resultado.HayErrores;
        }

        private bool ValidarRespuestaExitosaFerroviaria(confirmacionDefinitivaCPEFerroviariaResponse response, Resultado resultado)
        {
            return response?.respuesta?.cabecera?.estado == ESTADO_CONFIRMADO && !resultado.HayErrores;
        }

        private void GuardarPdfSiExiste(
            byte[] pdf,
            ConfirmarArriboDefinitivo comando,
            Recorrido recorrido,
            Centro centro)
        {
            if (pdf != null)
            {
                servicioComandos.Ejecutar(new GuardarImagenDescarga
                {
                    NroCartaPorte = comando.Dto.NroCartaPorte,
                    RutaFotoCP = comando.Dto.FotoRutaDestino,
                    CodigoCentroSap = centro.CodigoSAP,
                    Patente = recorrido.Patente,
                    TipoImagen = recorrido.Establecimiento != null ? TipoImagen.CPESustentable : TipoImagen.CPE,
                    Pdf = pdf
                });
            }
        }

        private void RegistrarLogAfip(string servicio, string consulta, string respuesta)
        {
            Repositorio.Agregar(new LogAfipCpe
            {
                Servicio = servicio,
                Consulta = consulta,
                Respuesta = respuesta,
                Fecha = DateTime.Now,
            });
        }

        private void ActualizarBajaCTGDefinitiva(ConfirmarArriboDefinitivo comando, Resultado resultado)
        {
            var bajaCtg = Repositorio.ObtenerMasReciente<BajaCTG>(
                x => x.WorkflowId == comando.WorkflowId,
                x => x.Fecha);

            if (bajaCtg != null && string.IsNullOrEmpty(bajaCtg.CodigoDeBajaDefinitivo))
                bajaCtg.CodigoDeBajaDefinitivo = !resultado.HayErrores ? nameof(ProcesadorConfirmacionArriboDefinitiva) : null;

            Repositorio.GuardarCambios();
        }

        private bool EsTipoCpeAutomotor(short tipoCpe)
        {
            return tipoCpe == TIPO_CPE_AUTOMOTOR;
        }

        private short ObtenerTipoCpe(TipoVehiculo tipoVehiculo)
        {
            switch (tipoVehiculo)
            {
                case TipoVehiculo.Camiones:
                case TipoVehiculo.Camión:
                case TipoVehiculo.CamiónC:
                case TipoVehiculo.CamiónD:
                case TipoVehiculo.CamiónE:
                case TipoVehiculo.Bitren:
                    return TIPO_CPE_AUTOMOTOR;

                case TipoVehiculo.Tren:
                case TipoVehiculo.Vapor:
                    return TIPO_CPE_FERROVIARIO;

                default:
                    return TIPO_CPE_AUTOMOTOR;
            }
        }

        private string LimpiarCuit(string cuit)
        {
            return !string.IsNullOrEmpty(cuit) ? cuit.Replace("-", string.Empty) : string.Empty;
        }
    }
}