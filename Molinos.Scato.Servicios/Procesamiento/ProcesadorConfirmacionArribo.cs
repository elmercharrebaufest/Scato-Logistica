using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
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
        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;
        public ProcesadorConfirmacionArribo(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCpe, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
        }

        public override Resultado Ejecutar(ConfirmarArribo comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////
           
            var resultado = new Resultado();

            try
            {
                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                if (centro == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Centro));
                }
                var transportista = Repositorio.Obtener<Transportista>(comando.Dto.TransportistaId ?? 0);
                if (transportista == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Transportista));
                }

                if (comando.Vehiculo == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Vehiculo));
                }

                Log.Debug("Creo la autorizacion");
                // Obtengo la autorizacion
                var cuitRepresentado = centro.Cuit != null ? centro.Cuit.Replace("-", string.Empty): string.Empty;
                var auth = accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);
                // Armo la consulta
                Log.Debug("armo consulta");
                var tipoCpe = ObtenerTipoCpe(comando.Dto.TipoVehiculo);
                if(tipoCpe == 74)
                {
                    var ctg = Convert.ToInt64(comando.Dto.NroCartaPorte);
                    var tipoCartaPorteElectronica = Repositorio.ObtenerProyeccion<CartaPorteElectronica, int?>(x => x.NroCTG == ctg, x => x.TipoCartaPorte);
                    tipoCpe = tipoCartaPorteElectronica is null ? tipoCpe : Convert.ToInt16(tipoCartaPorteElectronica);
                }

                var confirmarArriboRequest = new confirmarArriboCPERequest
                {
                    auth = auth,
                    solicitud = new ConfirmarArriboSolicitud
                    {
                        cuitSolicitante = long.Parse(comando.Dto.TitularCartaPorteCuil.Replace("-", "")),
                        cartaPorte = new AfipCPDigitalService.CartaPorte
                        {
                            nroOrden = int.Parse(comando.Dto.CTG),
                            sucursal = comando.Dto.Sucursal ?? 0,
                            tipoCPE = tipoCpe
                        }
                    }
                };

                Log.Debug("Inicio la consulta");
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //obtengo el estado actual
                if(tipoCpe == 74 || tipoCpe == 274)
                {
                    var consulta = serviceAfipCpe.consultarCPEAutomotor(new consultarCPEAutomotorRequest()
                    {
                        auth = auth,
                        solicitud = new ConsultarAutomotorSolicitud()
                        {
                            nroCTG = Convert.ToInt64(comando.Dto.NroCartaPorte),
                            nroCTGSpecified = true
                        }
                    });

                    if(consulta?.respuesta?.cabecera?.estado == "CF")
                    {
                        return resultado;
                    }
                }
                else
                {
                    var consulta = serviceAfipCpe.consultarCPEFerroviaria(new consultarCPEFerroviariaRequest()
                    {
                        auth = auth,
                        solicitud = new ConsultarFerroviariaSolicitud()
                        {
                            nroCTG = Convert.ToInt64(comando.Dto.NroCartaPorte),
                            nroCTGSpecified = true
                        }
                    });

                    if (consulta?.respuesta?.cabecera?.estado == "CF")
                    {
                        return resultado;
                    }
                }

                // Realizo la consulta
                var response = serviceAfipCpe.confirmarArriboCPE(confirmarArriboRequest);
                Log.Debug("Realizo la consulta ");

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {

                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = "ProcesadorConfirmacionArribo",
                            Fecha = DateTime.Now,
                            Comentario = confirmarArriboRequest.ToXml(),
                            NombreUsuario = "",
                            WorkflowInstanceId = comando.WorkflowId,
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
                                Servicio= "ConfirmarArribo",
                                Consulta= confirmarArriboRequest.ToXml(),
                                Respuesta= response.respuesta.ToXml(),
                                Fecha = DateTime.Now,
                        });

                    Log.Debug("Baja de ctg {0} procesada correctamente", comando.Dto.NroCartaPorte);
                }
                else
                {
                    Log.Error("Baja de ctg {0} respuesta invalida", comando.Dto.NroCartaPorte);
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo hacer la baja de CTG del codigo {0}", comando.Dto.NroCartaPorte);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo hacer la baja de CTG del codigo {0}", comando.Dto.NroCartaPorte);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }
            var bajaCtg = Repositorio.ObtenerMasReciente<BajaCTG>(x => x.WorkflowId == comando.WorkflowId, x => x.Fecha);
            if (bajaCtg == null)
            {
                Repositorio.Agregar(
                  new BajaCTG
                  {
                      CartaPorte = Repositorio.Obtener<Dominio.Entidades.CartaPorte>(comando.Dto.Id),
                      CodigoDeBaja = (!resultado.HayErrores) ? "ProcesadorConfirmacionArribo" : null,
                      Fecha = DateTime.Now,
                      WorkflowId = comando.WorkflowId
                  });
            }
            else {
                bajaCtg.CodigoDeBaja = (!resultado.HayErrores) ? "ProcesadorConfirmacionArribo" : null;
                bajaCtg.Fecha = DateTime.Now;
            }

            Repositorio.GuardarCambios();

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
    }
}