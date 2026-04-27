using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConfirmacionArribo : ProcesadorComando<ConfirmarArribo>
    {
        private const short TIPO_CPE_AUTOMOTOR = 74;
        private const short TIPO_CPE_FERROVIARIO = 75;
        private const string CONFIG_LOGUEAR_REQUESTS = "LoguearRequestsCtg";
        private const string CODIGO_ERROR_BAJA = "CodigoDeBaja";

        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorConfirmacionArribo(
            IRepositorio repositorio, 
            IConversor conversor, 
            ILogger log,
            CpePortType serviceAfipCpe, 
            IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
        }

        public override Resultado Ejecutar(ConfirmarArribo comando)
        {
            ConfigurarSeguridadSsl();
            var resultado = new Resultado();

            try
            {
                Validar(comando, resultado);
                if (resultado.HayErrores)
                    return resultado;
                
                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                var auth = ObtenerAutorizacion(centro, resultado);
                var tipoCpe = ObtenerTipoCpe(comando.Dto.TipoVehiculo);
                
                if (ValidarEstadoCpe(comando, tipoCpe, auth))
                {
                    Log.Debug("CPE {0} ya está confirmada, no se requiere acción", comando.Dto.NroCartaPorte);
                    return resultado;
                }

                var confirmarArriboRequest = ConstruirRequestConfirmacion(comando, auth, tipoCpe);
                RegistrarRequest(comando, confirmarArriboRequest);
                
                var response = EjecutarConfirmacionEnAfip(confirmarArriboRequest);
                ProcesarRespuestaAfip(response, resultado, confirmarArriboRequest, comando);
            }
            catch (FaultException ex)
            {
                Log.Error(ex, "Error de servicio AFIP al confirmar arribo de CTG: {0}", comando.Dto.NroCartaPorte);
                resultado.Errores.Add(CODIGO_ERROR_BAJA, "Error, el servicio de AFIP nos responde: " + ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al confirmar arribo de CTG: {0}", comando.Dto.NroCartaPorte);
                resultado.Errores.Add(CODIGO_ERROR_BAJA, Textos.Error_Generico);
            }
            finally
            {
                RegistrarBajaCTG(comando, resultado);
            }

            return resultado;
        }

        private void ConfigurarSeguridadSsl()
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        private void Validar(ConfirmarArribo comando, Resultado resultado)
        {
            if (comando.CentroId == 0)
                resultado.Error(nameof(ConfirmarArribo.CentroId), string.Format(Textos.Error_Requerido, Textos.Centro));

            if (!comando.Dto.TransportistaId.HasValue || comando.Dto.TransportistaId.Value == 0)
                resultado.Error(nameof(ConfirmarArribo.Dto.TransportistaId), string.Format(Textos.Error_Requerido, Textos.Transportista));

            if (comando.Vehiculo == null)
                resultado.Error(nameof(ConfirmarArribo.Vehiculo), string.Format(Textos.Error_Requerido, Textos.Vehiculo));
        }

        private Auth ObtenerAutorizacion(Centro centro, Resultado resultado)
        {
            Log.Debug("Obteniendo autorización AFIP para CUIT: {0}", centro.Cuit);
            var cuitRepresentado = LimpiarCuit(centro.Cuit);
            return accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);
        }

        private bool ValidarEstadoCpe(ConfirmarArribo comando, short tipoCpe, Auth auth)
        {
            var nroCTG = Convert.ToInt64(comando.Dto.NroCartaPorte);
            return tipoCpe == TIPO_CPE_AUTOMOTOR ? 
                ConsultarEstadoCpeAutomotor(nroCTG, auth) : 
                ConsultarEstadoCpeFerroviaria(nroCTG, auth);
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

            var estado = consulta?.respuesta?.cabecera?.estado;
            return EstadosCPEdeAFIP.ValidosParaConfirmacionArribo.Contains(estado);
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

            var estado = consulta?.respuesta?.cabecera?.estado;
            return EstadosCPEdeAFIP.ValidosParaConfirmacionArribo.Contains(estado);
        }

        private confirmarArriboCPERequest ConstruirRequestConfirmacion(ConfirmarArribo comando, Auth auth, short tipoCpe)
        {
            return new confirmarArriboCPERequest
            {
                auth = auth,
                solicitud = new ConfirmarArriboSolicitud
                {
                    cuitSolicitante = long.Parse(LimpiarCuit(comando.Dto.TitularCartaPorteCuil)),
                    cartaPorte = new AfipCPDigitalService.CartaPorte
                    {
                        nroOrden = int.Parse(comando.Dto.CTG),
                        sucursal = comando.Dto.Sucursal ?? 0,
                        tipoCPE = tipoCpe
                    }
                }
            };
        }

        private confirmarArriboCPEResponse EjecutarConfirmacionEnAfip(confirmarArriboCPERequest request)
        {
            Log.Debug("Ejecutando confirmación de arribo en AFIP");
            return serviceAfipCpe.confirmarArriboCPE(request);
        }

        private void RegistrarRequest(ConfirmarArribo comando, confirmarArriboCPERequest request)
        {
            try
            {
                if (ConfigurationManager.AppSettings[CONFIG_LOGUEAR_REQUESTS] == "1")
                {
                    Repositorio.Agregar(new ControlRecorrido
                    {
                        Actividad = nameof(ProcesadorConfirmacionArribo),
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
            confirmarArriboCPEResponse response, 
            Resultado resultado, 
            confirmarArriboCPERequest request, 
            ConfirmarArribo comando)
        {
            if (ValidarRespuestaError(response))
            {
                var error = response.respuesta.errores.FirstOrDefault();
                resultado.Errores.Add(error.codigo, error.descripcion);
                Log.Error("Error en la confirmación: {0}", error.descripcion);
            }
            else if (ValidarRespuestaExitosa(response, resultado))
            {
                Repositorio.Agregar(new LogAfipCpe
                {
                    Servicio = "ConfirmarArribo",
                    Consulta = request.ToXml(),
                    Respuesta = response.respuesta.ToXml(),
                    Fecha = DateTime.Now,
                });
                Log.Debug("Confirmación de arribo procesada correctamente para CTG: {0}", comando.Dto.NroCartaPorte);
            }
            else
            {
                Log.Error("Respuesta inválida de AFIP para CTG: {0}", comando.Dto.NroCartaPorte);
            }
        }

        private bool ValidarRespuestaError(confirmarArriboCPEResponse response)
        {
            return response.respuesta != null && 
                   response.respuesta.errores != null && 
                   response.respuesta.errores.Any();
        }

        private bool ValidarRespuestaExitosa(confirmarArriboCPEResponse response, Resultado resultado)
        {
            return response.respuesta != null && !resultado.HayErrores;
        }

        private void RegistrarBajaCTG(ConfirmarArribo comando, Resultado resultado)
        {
            var bajaCtg = Repositorio.ObtenerMasReciente<BajaCTG>(
                x => x.WorkflowId == comando.WorkflowId, 
                x => x.Fecha);

            if (bajaCtg == null)
            {
                Repositorio.Agregar(new BajaCTG
                {
                    CartaPorte = Repositorio.Obtener<Dominio.Entidades.CartaPorte>(comando.Dto.Id),
                    CodigoDeBaja = !resultado.HayErrores ? nameof(ProcesadorConfirmacionArribo) : null,
                    Fecha = DateTime.Now,
                    WorkflowId = comando.WorkflowId
                });
            }
            else
            {
                bajaCtg.CodigoDeBaja = !resultado.HayErrores ? nameof(ProcesadorConfirmacionArribo) : null;
                bajaCtg.Fecha = DateTime.Now;
            }
            
            Repositorio.GuardarCambios();
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