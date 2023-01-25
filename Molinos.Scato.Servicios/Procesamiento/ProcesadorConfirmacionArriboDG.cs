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
using System.Linq;
using System.Net;
using System.ServiceModel;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConfirmacionArriboDG : ProcesadorComando<ConfirmarArriboDG>
    {
        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorConfirmacionArriboDG(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCpe, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
        }

        public override Resultado Ejecutar(ConfirmarArriboDG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new Resultado();

            try
            {
                //Recorrido Id
                var recorrido = Repositorio.Obtener<Recorrido>(f => (f.Id == comando.Dto.RecorridoId));
                //var centro = Repositorio.Obtener<Centro>(f => (f.Id == recorrido.Id));
                if (recorrido.Centro == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Centro));
                }

                Log.Debug("Creo la autorizacion");
                // Obtengo la autorizacion
                var cuitRepresentado = recorrido.Centro.Cuit != null ? recorrido.Centro.Cuit.Replace("-", string.Empty) : string.Empty;
                var auth = accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);

                // Armo la consulta
                Log.Debug("armo consulta");
                //var tipoCpe = ObtenerTipoCpe(comando.Dto.TipoVehiculo);
                //if (tipoCpe == 74)
                //{
                //    var ctg = Convert.ToInt64(comando.Dto.NumeroCTG);
                //    var tipoCartaPorteElectronica = Repositorio.ObtenerProyeccion<CartaPorteElectronica, int?>(x => x.NroCTG == ctg, x => x.TipoCartaPorte);
                //    tipoCpe = tipoCartaPorteElectronica is null ? tipoCpe : Convert.ToInt16(tipoCartaPorteElectronica);
                //}

                consultarCPEAutomotorDGResponse consulta = new consultarCPEAutomotorDGResponse();

                Log.Debug("Inicio la consulta");
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

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

                if (consulta?.respuesta == null || consulta?.respuesta?.errores.Any() == true)
                {
                    return resultado;
                }

                if (consulta?.respuesta?.cabecera?.estado == "CF")
                {
                    return resultado;
                }

                var confirmarArriboRequest = new confirmarArriboCPERequest
                {
                    auth = auth,
                    solicitud = new ConfirmarArriboSolicitud
                    {
                        cuitSolicitante = consulta.respuesta.origen.cuitOrigen,
                        cartaPorte = new AfipCPDigitalService.CartaPorte
                        {
                            nroOrden = (int)consulta.respuesta.cabecera.nroOrden,
                            sucursal = consulta.respuesta.cabecera.sucursal,
                            tipoCPE = (short)consulta.respuesta.cabecera.tipoCartaPorte,
                        }
                    }
                };
                // Realizo la consulta
                var response = serviceAfipCpe.confirmarArriboCPE(confirmarArriboRequest);
                Log.Debug("Realizo la consulta ");

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {
                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = "ProcesadorConfirmacionArriboDG",
                            Fecha = DateTime.Now,
                            Comentario = confirmarArriboRequest.ToXml(),
                            NombreUsuario = "",
                            WorkflowInstanceId = recorrido.InstanciaWorkflow,
                        });
                        Repositorio.GuardarCambios();
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("Error al loguear request Afip CTG", e.Message);
                }

                if (response.respuesta != null && response.respuesta.errores != null && response.respuesta.errores.Any())
                {
                    resultado.Errores.Add(response.respuesta.errores.FirstOrDefault().codigo, response.respuesta.errores.FirstOrDefault().descripcion);
                    Log.Error("Error en la Confirmacion: {0}", response.respuesta.errores.FirstOrDefault().descripcion);
                }
                else if (response.respuesta != null && !resultado.HayErrores)
                {
                    //Si no hay errores, registro la baja del CTG
                    var datos = response.respuesta.cabecera;
                    Repositorio.Agregar(
                        new LogAfipCpe
                        {
                            Servicio = "ConfirmarArribo",
                            Consulta = confirmarArriboRequest.ToXml(),
                            Respuesta = response.respuesta.ToXml(),
                            Fecha = DateTime.Now,
                        });

                    Log.Debug("Baja de ctg {0} procesada correctamente", comando.Dto.NumeroCTG);
                }
                else
                {
                    Log.Error("Baja de ctg {0} respuesta invalida", comando.Dto.NumeroCTG);
                }

                var bajaCtg = Repositorio.ObtenerMasReciente<BajaCTG>(x => x.WorkflowId == recorrido.InstanciaWorkflow, x => x.Fecha);
                if (bajaCtg == null)
                {
                    Repositorio.Agregar(
                      new BajaCTG
                      {
                          CartaPorte = Repositorio.Obtener<Dominio.Entidades.CartaPorte>(comando.Dto.Id),
                          CodigoDeBaja = (!resultado.HayErrores) ? "ProcesadorConfirmacionArribo" : null,
                          Fecha = DateTime.Now,
                          WorkflowId = recorrido.InstanciaWorkflow
                      });
                }
                else
                {
                    bajaCtg.CodigoDeBaja = (!resultado.HayErrores) ? "ProcesadorConfirmacionArribo" : null;
                    bajaCtg.Fecha = DateTime.Now;
                }

                Repositorio.GuardarCambios();
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo hacer la baja de CTG del codigo {0}", comando.Dto.NumeroCTG);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo hacer la baja de CTG del codigo {0}", comando.Dto.NumeroCTG);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }

            return resultado;
        }
    }
}