using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
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
using ControlRecorrido = Molinos.Scato.Dominio.Entidades.ControlRecorrido;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConfirmacionArriboDGDefinitiva : ProcesadorComando<ConfirmarArriboDGDefinitivo>
    {
        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;
        private IServicioComandos servicioComandos;

        public ProcesadorConfirmacionArriboDGDefinitiva(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCpe, IAccesoWsCtg accesoWsCtg, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ConfirmarArriboDGDefinitivo comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new Resultado();

            try
            {
                var recorrido = Repositorio.Obtener<Recorrido>(f => (f.Id == comando.Dto.RecorridoId));

                //var centro = Repositorio.Obtener<Centro>(recorrido.Id);
                if (recorrido.Centro == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Centro));
                }
                

                Log.Debug("Creo la autorizacion");
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                // Obtengo la autorizacion
                var cuitRepresentado = recorrido.Centro.Cuit != null ? recorrido.Centro.Cuit.Replace("-", string.Empty) : string.Empty;
                var auth = accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);
                // Armo la consulta
                Log.Debug("armo consulta dependiendo del tipo de vehiculo");
                var request = "";

                consultarCPEAutomotorDGResponse consulta = new consultarCPEAutomotorDGResponse();

                //obtengo el estado actual

                consulta = serviceAfipCpe.consultarCPEAutomotorDG(new consultarCPEAutomotorDGRequest()
                {
                    auth = auth,
                    solicitud = new ConsultarAutomotorDGSolicitud()
                    {
                        nroCTG = Convert.ToInt64(comando.Dto.NumeroCTG),
                        nroCTGSpecified = true
                    }
                });

                if (consulta?.respuesta?.cabecera?.estado == "CN")
                {
                    UpdateBajaCTGDefinitiva(recorrido.InstanciaWorkflow);
                    return resultado;
                }

                if (consulta?.respuesta?.datosCarga?.pesoBruto == null || consulta?.respuesta?.datosCarga?.pesoBruto == null)
                {
                    Log.Error("ProcesadorConfirmacionDGArriboDefinitivo - PESO NO ENCONTRADO");
                    resultado.Errores.Add("CodigoDeBaja", "No se puede ejecutar la confirmacion definitiva de un camión sin peso");
                    return resultado;
                }

                //if (tipoCpe == 74)
                //{
                //    var ctg = Convert.ToInt64(comando.Dto.NumeroCTG);
                //    var tipoCartaPorteElectronica = Repositorio.ObtenerProyeccion<CartaPorteElectronica, int?>(x => x.NroCTG == ctg, x => x.TipoCartaPorte);
                //    tipoCpe = tipoCartaPorteElectronica is null ? tipoCpe : Convert.ToInt16(tipoCartaPorteElectronica);
                //}

                var confirmarArriboRequest = new confirmacionDefinitivaCPEAutomotorDGRequest
                {
                    auth = auth,
                    solicitud = new ConfirmacionAutomotorDGSolicitud
                    {
                        cuitSolicitante = consulta.respuesta.origen.cuitOrigen,
                        pesoBrutoDescarga = recorrido.PesoBruto ?? 0,
                        pesoTaraDescarga = recorrido.PesoTara ?? 0,
                        cartaPorte = new AfipCPDigitalService.CartaPorte
                        {
                            nroOrden = (int)consulta.respuesta.cabecera.nroOrden,
                            sucursal = consulta.respuesta.cabecera.sucursal,
                            tipoCPE = (short)consulta.respuesta.cabecera.tipoCartaPorte
                        },
                    }
                };
                request = confirmarArriboRequest.ToXml();
                Log.Debug("Inicio la consulta");
                // Realizo la consulta
                var respuesta = serviceAfipCpe.confirmacionDefinitivaCPEAutomotorDG(confirmarArriboRequest).respuesta;
                //if(respuesta.pdf != null) // TODO - Revisar si es necesario, ya que PDF actualmente siempre es null
                //{
                //    servicioComandos.Ejecutar(new GuardarImagenDescarga
                //    {
                //        NroCartaPorte = comando.Dto.NumeroCTG,
                //        RutaFotoCP = comando.Dto.FotoRutaDestino,
                //        CodigoCentroSap = centro.CodigoSAP,
                //        Patente = recorrido.Patente,
                //        EsSustentable = recorrido.Establecimiento != null,
                //        Pdf = respuesta.pdf
                //    });
                //}
                Log.Debug("Realizo la consulta ");

                Repositorio.Agregar(
                new LogAfipCpe
                {
                    Servicio = "ConfirmarArriboDGDefinitivo",
                    Consulta = request,
                    Respuesta = respuesta.ToXml(),
                    Fecha = DateTime.Now,
                });

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {
                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = "ProcesadorConfirmacionArriboDGDefinitivo",
                            Fecha = DateTime.Now,
                            Comentario = request,
                            NombreUsuario = "",
                            WorkflowInstanceId = recorrido.InstanciaWorkflow,
                        });
                        Repositorio.GuardarCambios();
                    }

                    // UpdateBajaCTGDefinitiva(comando.WorkflowId);
                }
                catch (Exception e)
                {
                    Log.Debug("Error al loguear request Afip CTG", e.Message);
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo hacer la Confirmacion Definitiva del codigo ctg {0} ", comando.Dto.NumeroCTG);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo hacer la Confirmacion Definitiva del codigo ctg {0} ", comando.Dto.NumeroCTG);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }

        private short ObtenerTipoCpe(Dominio.Enums.TipoVehiculo tipoVehiculo)
        {
            switch (tipoVehiculo)
            {
                case Dominio.Enums.TipoVehiculo.Camiones:
                case Dominio.Enums.TipoVehiculo.Camión:
                case Dominio.Enums.TipoVehiculo.CamiónC:
                case Dominio.Enums.TipoVehiculo.CamiónD:
                case Dominio.Enums.TipoVehiculo.CamiónE:
                case Dominio.Enums.TipoVehiculo.Bitren:
                    return 74;

                case Dominio.Enums.TipoVehiculo.Tren:
                case Dominio.Enums.TipoVehiculo.Vapor:
                    return 75;

                default:
                    return 74;
            }
        }

        private void UpdateBajaCTGDefinitiva(Guid workFlowId)
        {
            var bajaCtg = Repositorio.ObtenerMasReciente<BajaCTG>(x => x.WorkflowId == workFlowId, x => x.Fecha);
            if (bajaCtg != null)
            {
                if (string.IsNullOrEmpty(bajaCtg.CodigoDeBajaDefinitivo))
                    bajaCtg.CodigoDeBajaDefinitivo = "ProcesadorConfirmacionArriboDefinitivo";
            }
            Repositorio.GuardarCambios();
        }
    }
}