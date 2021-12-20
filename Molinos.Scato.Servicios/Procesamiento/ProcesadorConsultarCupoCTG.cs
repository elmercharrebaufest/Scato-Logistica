using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarCupoCTG : ProcesadorComando<ConsultarCupoCTG>
    {
        const int cLongitudMaximaCupo = 16; //Se establece el limite maximo de caracteres para el campo cupo de la consulta de CTG.
        private CTGServicePortType serviceAfipCTG;
        private IAccesoWsCtg accesoWsCtg;
        private IKernel kernel;

        public ProcesadorConsultarCupoCTG(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CTGServicePortType serviceAfipCTG, IAccesoWsCtg accesoWsCtg, IKernel kernel)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.kernel = kernel;
            this.serviceAfipCTG = serviceAfipCTG;
        }

        public override Resultado Ejecutar(ConsultarCupoCTG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new ResultadoDetalleCTG();

            var centro = Repositorio.Obtener<Centro>(comando.CentroId);

            try
            {
                Log.Debug("ProcesadorConsultarCupoCTG - Creo la autorizacion");
                // Obtengo la autorizacion
                var auth = accesoWsCtg.ObtenerAuthType(centro.Cuit.Replace("-", string.Empty), resultado);
                // Armo la consulta
                var request = new consultarCTGRequest(new consultarCTGRequestType
                {
                    auth = auth,
                    consultarCTGDatos = new consultarCTGDatosType
                    {
                        cartaPorte = Convert.ToInt64(comando.NumeroCartaPorte),
                        fechaEmisionDesde = DateTime.Now.AddMonths(-3).ToString("dd/MM/yyyy"),
                        cartaPorteSpecified = true
                    }
                });
                Log.Debug(request.ToXml());
                var responseCtg = serviceAfipCTG.consultarCTG(request);

                if (responseCtg.response != null && responseCtg.response.arrayErrores != null && responseCtg.response.arrayErrores.Any())
                {
                    resultado.Errores.Add("2", responseCtg.response.arrayErrores.FirstOrDefault());
                    Log.Error("ProcesadorConsultarCupoCTG -" + responseCtg.response.arrayErrores.FirstOrDefault());
                }
                else if (responseCtg.response != null)
                {
                    var respuestaCtg = responseCtg.response.arrayDatosConsultarCTG;

                    Log.Debug("ProcesadorConsultarCupoCTG - Inicio la consulta");
                    // Realizo la consulta
                    var response = serviceAfipCTG.consultarDetalleCTG(new consultarDetalleCTGRequest(new consultarDetalleCTGRequestType
                    {
                        auth = auth,
                        ctg = Convert.ToInt64(respuestaCtg[0].ctg.ToString().Replace(".", string.Empty))
                    }
                    ));
                    Log.Debug("ProcesadorConsultarCupoCTG - Realizo la consulta ");

                    if (response.response != null && response.response.arrayErrores != null && response.response.arrayErrores.Any())
                    {
                        resultado.Errores.Add("2", response.response.arrayErrores.FirstOrDefault());
                        Log.Error("ProcesadorConsultarCupoCTG -" + response.response.arrayErrores.FirstOrDefault());
                    }
                    else if (response.response.consultarDetalleCTGDatos.estado == "Anulado" || response.response.consultarDetalleCTGDatos.estado == "Rechazado")
                    {
                        resultado.Errores.Add("2", "Consulta Detalle CTG - CTG Anulado o Rechazado");
                        Log.Error("ProcesadorConsultaCupoCTG - CTG {0} Anulado o Rechazado ", response.response.consultarDetalleCTGDatos.ctg);
                    }
                    else if (response.response != null)
                    {
                        var respuesta = response.response.consultarDetalleCTGDatos;
                        var materiales = Repositorio.Listar<Material>(x => x.DescripcionCorta == respuesta.especie);
                        var entidad = new CartaPorte
                        {
                            CTG = respuestaCtg[0].ctg.ToString(),
                            CentroDestino = centro,
                            Cupo = respuesta.turno == null ? null : (respuesta.turno.Length <= cLongitudMaximaCupo ? respuesta.turno : respuesta.turno.Substring(0, cLongitudMaximaCupo)),
                            NroCartaPorte = respuesta.cartaPorte.ToString(CultureInfo.InvariantCulture).PadLeft(12, '0'),
                            Material = materiales.Count == 1 ? materiales.First() : null,
                            CodEstab = respuesta.establecimiento.ToString(CultureInfo.InvariantCulture),
                            RtteComercial = ObtenerProveedor(respuesta.cuitCanjeador, resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                            TitularCartaPorte = ObtenerProveedor(respuesta.solicitante, resultado, Textos.CartaPorte_TitularCartaPorte, false, false, true)
                        };
                        //Fin Respuesta obtenida Detalle_CTG
                        resultado.CartaPorte = Conversor.Convertir<CartaPorte, CartaPorteDto>(entidad);
                        resultado.CartaPorte.Patente = respuesta.patenteVehiculo;
                        Log.Debug($"Consulta de Cupo CTG {respuestaCtg[0].ctg.ToString()} cp {comando.NumeroCartaPorte} procesada correctamente");
                        //resultado.CartaPorte = entidad;
                    }
                    else
                    {
                        resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                        Log.Debug($"Consulta de Cupo CTG {respuestaCtg[0].ctg.ToString()} cp {comando.NumeroCartaPorte} sin respuesta");
                    }
                }
                else
                {
                    resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                    Log.Debug($"Consulta de Cupo cp {comando.NumeroCartaPorte} sin respuesta");
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo consultar el Cupo por ctg {0}", comando.NumeroCartaPorte);
                resultado.Errores.Add("2", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo consultar el Cupo por ctg {0}", comando.NumeroCartaPorte);
                resultado.Errores.Add("2", Textos.Error_Generico);
            }
            return resultado;
        }

        private Proveedor ObtenerProveedor(string cuitSinGuiones, Resultado resultado, string nombreDato, bool am, bool cm, bool pr)
        {
            if (!string.IsNullOrEmpty(cuitSinGuiones))
            {
                var cuil = cuitSinGuiones.Substring(0, 2) + "-" + cuitSinGuiones.Substring(2, 8) + "-" + cuitSinGuiones.Substring(10, 1);
                var proveedor = Repositorio.Listar<Proveedor>(x => x.Cuil == cuil && ((x.AM == am && am) || (x.CM == cm && cm) || (x.PR == pr && pr)) && x.Activo).LastOrDefault();
                if (proveedor == null)
                {
                    try
                    {
                        kernel.Get<ProcesadorSincronizarProveedores>().Ejecutar(new SincronizarProveedores { RetornarResultado = false, Cuit = cuil, CargaMasiva = false });
                        proveedor = Repositorio.Listar<Proveedor>(x => x.Cuil == cuil && ((x.AM == am && am) || (x.CM == cm && cm) || (x.PR == pr && pr)) && x.Activo).LastOrDefault();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "No se pudo consultar el {0} {1} en sap", nombreDato, cuil);
                    }
                }
                if (proveedor == null)
                {
                    Log.Warn("No se pudo encontrar el {0} {1} en scato ni en sap", nombreDato, cuil);
                    //AgregarWarning(resultado, string.Format(Textos.Proveedor_NoEncontrado, nombreDato, cuil));
                }
                return proveedor;
            }
            return null;
        }

        private void AgregarWarning(Resultado resultado, string mensaje)
        {
            if (resultado.Errores.ContainsKey("1"))
            {
                resultado.Errores["1"] += "<br>" + mensaje;
            }
            else
            {
                resultado.Errores.Add("1", mensaje);
            }
        }


    }
}
