using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarDetalleCTG : ProcesadorComando<ConsultarDetalleCTG>
    {
        const int cLongitudMaximaCupo = 16; //Se establece el limite maximo de caracteres para el campo cupo de la consulta de CTG.
        private CTGServicePortType serviceAfipCTG;
        private IAccesoWsCtg accesoWsCtg;
        private IKernel kernel;

        public ProcesadorConsultarDetalleCTG(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CTGServicePortType serviceAfipCTG, IAccesoWsCtg accesoWsCtg, IKernel kernel)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.kernel = kernel;
            this.serviceAfipCTG = serviceAfipCTG;
        }

        public override Resultado Ejecutar(ConsultarDetalleCTG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new ResultadoDetalleCTG();

            var centro = Repositorio.Obtener<Centro>(comando.CentroId);

            try
            {
                Log.Debug("ProcesadorConsultarDetalleCTG - Creo la autorizacion");
                // Obtengo la autorizacion
                var auth = accesoWsCtg.ObtenerAuthType(centro.Cuit.Replace("-", string.Empty), resultado);
                // Armo la consulta
                ConsultarDetalle(centro, comando.Ctg, resultado, auth);
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo consultar el ctg {0}", comando.Ctg);
                resultado.Errores.Add("2", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo consultar el ctg {0}", comando.Ctg);
                resultado.Errores.Add("2", Textos.Error_Generico);
            }
            return resultado;
        }

        private void ConsultarDetalle(Centro centro, string ctg, ResultadoDetalleCTG resultado, authType auth)
        {
            Log.Debug("ProcesadorConsultarDetalleCTG - armo consulta");
            var consultarCTGRequest = new consultarDetalleCTGRequestType
            {
                auth = auth,
                ctg = Convert.ToInt64(ctg)
            };

            Log.Debug("ProcesadorConsultarDetalleCTG - Inicio la consulta");
            // Realizo la consulta
            var response = serviceAfipCTG.consultarDetalleCTG(new consultarDetalleCTGRequest { request = consultarCTGRequest });
            Log.Debug("ProcesadorConsultarDetalleCTG - Realizo la consulta ");
            var resEstado = response.response.consultarDetalleCTGDatos;
            
            if (response.response != null && response.response.arrayErrores != null && response.response.arrayErrores.Any())
            {
                resultado.Errores.Add("2", response.response.arrayErrores.FirstOrDefault());
                Log.Error("ProcesadorConsultarDetalleCTG -" + response.response.arrayErrores.FirstOrDefault());
            }
            else if(resEstado.estado == "Anulado" || resEstado.estado == "Rechazado")
            {
                resultado.Errores.Add("2", "Consulta Detalle CTG - CTG Anulado o Rechazado");
                Log.Error("ProcesadorConsultarDetalleCTG - CTG {0} Anulado o Rechazado ", resultado.CartaPorte.CTG);
            }
            else if (response.response != null)
            {
                //Si no hay errores, registro la Consulta detalle del CTG
                var respuesta = response.response.consultarDetalleCTGDatos;
                var localidad = string.IsNullOrEmpty(respuesta.localidadOrigen) ? "" : respuesta.localidadOrigen.Split('(')[0].TrimEnd();
                var provincia = string.IsNullOrEmpty(respuesta.localidadOrigen) ? "" : respuesta.localidadOrigen.Split('(')[1].Replace(")", "");
                var localidadObj = Repositorio.Listar<Localidad>(x => x.Descripcion == localidad && x.Provincia.Descripcion == provincia).FirstOrDefault();
                Localidad localidadDto = null;
                if (localidadObj != null)
                {
                    localidadDto = new Localidad
                    {
                        Id = localidadObj.Id,
                        Provincia = new Provincia {Id= localidadObj.Provincia.Id, CodigoAfip= localidadObj.Provincia.CodigoAfip, Descripcion= localidadObj.Provincia.Descripcion },
                        CodigoAfip = localidadObj.CodigoAfip,
                        Descripcion = localidadObj.Descripcion
                    };
                    localidadObj.Descripcion = localidadObj.CodigoAfip + " - " + localidadObj.Descripcion + "(" + provincia + ")";
                }

                var localidadDestino = string.IsNullOrEmpty(respuesta.localidadDestino) ? "" : respuesta.localidadDestino.Split('(')[0].TrimEnd();
                var provinciaDestino = string.IsNullOrEmpty(respuesta.localidadDestino) ? "" : respuesta.localidadDestino.Split('(')[1].Replace(")", "");

                var localidadDestinoObj = Repositorio.Listar<Localidad>(x => x.Descripcion == localidadDestino && x.Provincia.Descripcion == provinciaDestino).FirstOrDefault();
                if (localidadDestinoObj == null || localidadDestinoObj.Id != centro.Localidad.Id)
                {
                    AgregarWarning(resultado, string.Format(Textos.Error_Ctg_Localidad, localidadDestino + "-" + provinciaDestino));
                }

                var materiales = Repositorio.Listar<Material>(x => x.DescripcionCorta == respuesta.especie && respuesta.especie != "Girasol");
                //Respuesta obtenida Detalle_CTG
                var entidad = new CartaPorte
                {
                    CTG = ctg,
                    TitularCartaPorte = ObtenerProveedor(respuesta.solicitante, resultado, Textos.CartaPorte_TitularCartaPorte, false, false, true),
                    NroCartaPorte = respuesta.cartaPorte.ToString(CultureInfo.InvariantCulture).PadLeft(12, '0'),
                    FechaEmision = DateTime.Now,
                    FechaCP = string.IsNullOrEmpty(respuesta.fechaEmision) ? DateTime.MinValue : DateTime.ParseExact(respuesta.fechaEmision.Split(' ')[0], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    //FechaVto = string.IsNullOrEmpty(respuesta.fechaVigenciaHasta) ? DateTime.MinValue : DateTime.ParseExact(respuesta.fechaVigenciaHasta.Split(' ')[0], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Material = materiales.Count == 1 ? materiales.First() : null,
                    CentroDestino = centro,
                    //CodEstab = respuesta.establecimiento.ToString(CultureInfo.InvariantCulture),
                    Destinatario = ObtenerProveedor(respuesta.cuitDestinatario, resultado, Textos.CartaPorte_Destinatario, false, false, true),
                    Procedencia = localidadObj,
                    Cosecha = respuesta.cosecha,
                    Transportista = ObtenerTransportista(respuesta.cuitTransportista, resultado),
                    Vehiculos = new List<Vehiculo>
                        {
                           new Vehiculo
                           {
                               Patente = respuesta.patenteVehiculo,
                               //PesoBrutoOrigen = unchecked((int)respuesta.pesoNetoCarga),
                               //PesoTaraOrigen = 0,
                               //PesoNetoOrigen = unchecked((int)respuesta.pesoNetoCarga)
                           }
                        },
                    KmRecorrer = unchecked((int)respuesta.kmARecorrer),
                    TarifaReferencia = respuesta.tarifaReferencia,
                    Corredor = ObtenerProveedor(respuesta.cuitCorredor, resultado, Textos.CartaPorte_CorredorVendedor, false, true, false),
                    Cupo = respuesta.turno == null ? null : (respuesta.turno.Length <= cLongitudMaximaCupo ? respuesta.turno : respuesta.turno.Substring(0, cLongitudMaximaCupo)),
                    RtteComercial = ObtenerProveedor(respuesta.cuitCanjeador, resultado, Textos.CartaPorte_RtteComercial, false, false, true)
                };
                //Fin Respuesta obtenida Detalle_CTG
                resultado.CartaPorte = Conversor.Convertir<CartaPorte, CartaPorteDto>(entidad);
                Log.Debug("Consulta de CTG {0} procesada correctamente", ctg);
                //resultado.CartaPorte = entidad;
            }
            else
            {
                resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                Log.Debug("Consulta de CTG {0} sin respuesta", ctg);
            }
        }

        private Transportista ObtenerTransportista(string cuitTransportista, Resultado resultado)
        {
            if (!string.IsNullOrEmpty(cuitTransportista))
            {
                var cuil = cuitTransportista.Substring(0, 2) + "-" + cuitTransportista.Substring(2, 8) + "-" + cuitTransportista.Substring(10, 1);
                var transportista = Repositorio.Listar<Transportista>(x => x.Cuit == cuil).LastOrDefault();
                if(transportista == null)
                {
                    AgregarWarning(resultado, string.Format(Textos.Transportista_NoEncontrado, cuil));
                }
                return transportista;
            }
            return null;
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
                    AgregarWarning(resultado, string.Format(Textos.Proveedor_NoEncontrado, nombreDato, cuil));
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
