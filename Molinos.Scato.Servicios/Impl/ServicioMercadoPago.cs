using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;


namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioMercadoPago : IServicioMercadoPago
    {
        private readonly IRepositorio repositorio;
        private readonly HttpClient httpClient;
        private readonly ILogger log;

        private readonly string urlPagoMercadoPago = ConfigurationManager.AppSettings["UrlPagoMercadoPago"];
        private readonly string urlDevolucionMercadoPago = ConfigurationManager.AppSettings["UrlDevolucionMercadoPago"];
        private readonly string urlSolicitarCredenciales = ConfigurationManager.AppSettings["UrlSolicitarCredenciales"];
        private readonly string urlCaja = ConfigurationManager.AppSettings["UrlCaja"];
        public ServicioMercadoPago(IRepositorio repositorio, ILogger log, HttpClient httpClient)
        {
            this.repositorio = repositorio;
            this.httpClient = httpClient;
            this.log = log;
        }

        public EstadoPagoDto Pagar(double montoACobrar, string tokenDePago, string idempotencia, string garita, int puestoDeTrabajoId)
        {
            var vendedor = ObtenerCredenciales(garita, puestoDeTrabajoId);
            var garitaObj = vendedor.GaritasDeSalida.First(x => x.PuestoDeTrabajo.Id == puestoDeTrabajoId);
            string urlBase = string.Format(urlPagoMercadoPago, vendedor.UsuarioVendedorId ?? 0, garitaObj.PuestoDeTrabajo.Id) + vendedor.AccessToken;

            var pago = new PagoConQrDto
            {
                Descripcion = "Pago impuesto municipal",
                ExternalReference = idempotencia,
                Nombre = "Pago impuesto municipal",
                TokenDePago = tokenDePago,
                MontoCobrado = montoACobrar,
                Items = new List<ItemDto>
                {
                    new ItemDto
                    {
                        Cantidad = 1,
                        Descripcion = "Pago impuesto municipal",
                        MontoCobrado = montoACobrar,
                        Nombre = "Pago impuesto municipal",
                        Unidad =montoACobrar,
                        UnidadDeMedida = "unit"
                    }
                }
            };
            var errorTexto = string.Empty;
            EstadoPagoDto estadoDePagoRealizado = null;
            try
            {
                var detalleDePago = Post<DetalleDePagoDto>(urlBase, pago, idempotencia);
                estadoDePagoRealizado = detalleDePago.PagosRealizados.First();

                if (detalleDePago.Estado == "rejected" || (estadoDePagoRealizado.Estado == "rejected"))
                {
                    log.Error($"Error MP: {idempotencia}" + detalleDePago.ToXml());
                    errorTexto = ObtenerDescripcionDelError(0, estadoDePagoRealizado.DetalleDeEstado);
                }
            }
            catch(MercadoLibreException e)
            {
                errorTexto = e.Message;
            }
            
            return (string.IsNullOrEmpty(errorTexto)) ? estadoDePagoRealizado : new EstadoPagoDto { Error = $"Cobro rechazado: {errorTexto}" };
        }

        public DetalleDeDevolucionDto Reembolsar(string mercadopagoId, string garita, int puestoDeTrabajoId)
        {
            var marketPlace = repositorio.ObtenerMayor<CredencialMercadoPago, int>(m => m.AppId != null, x => x.Id);
            if (marketPlace == null)
            {
                throw new Exception("No se encontró ningún MarketPlace y/o Vendedor");
            }; string urlBase = string.Format(urlDevolucionMercadoPago, mercadopagoId) + marketPlace.AccessToken;
            return Post<DetalleDeDevolucionDto>(urlBase, null);
        }
        public string SolicitarCredencialesVendedor(string codigoObtenido)
        {
            var credencialesMarketPlace = repositorio.ObtenerMayor<CredencialMercadoPago, int>(m => m.AppId != null, x => x.Id);

            if (credencialesMarketPlace == null)
            {
                throw new Exception("No se registro ningún MarketPlace");
            }

            var solicitud = new CredencialMercadoPagoDto
            {
                AppId = credencialesMarketPlace.AppId,
                SecretKey = credencialesMarketPlace.SecretKey,
                TipoDeSolicitud = "authorization_code",
                CodigoSolicitudDePermiso = codigoObtenido,
                RedirectUri = credencialesMarketPlace.RedirectUri
            };

            var credenciales = Post<CredencialMercadoPagoDto>(urlSolicitarCredenciales, solicitud);
            return (GuardarCredencial(credenciales)) ?
                "Credenciales Guardadas Exitosamente" : throw new Exception("Hubo un error al Guardar las Credenciales");
        }
                
        private CredencialMercadoPago ObtenerCredenciales(string garita, int puestoDeTrabajoId)
        {
            var vendedor = repositorio.ObtenerMayor<CredencialMercadoPago, int>(v => v.UsuarioVendedorId != null, x => x.Id);

            if (vendedor == null)
            {
                throw new Exception("No se encontró ningún MarketPlace y/o Vendedor");
            };
            
            if ((vendedor.FechaVencimientoPermisos - DateTime.Now).Value.Days <= 30)
            {
                var marketPlace = repositorio.ObtenerMayor<CredencialMercadoPago, int>(m => m.AppId != null, x => x.Id);
                if (marketPlace == null)
                {
                    throw new Exception("No se encontró ningún MarketPlace y/o Vendedor");
                };

                RenovarCredencialesVendedor(marketPlace.AccessToken, vendedor.TokenActualizarPermiso);
            };
            if (!string.IsNullOrEmpty(garita))
            {
                if (vendedor.GaritasDeSalida == null || !vendedor.GaritasDeSalida.Any(x => x.PuestoDeTrabajo.Id == puestoDeTrabajoId))
                {
                    ObtenerOCrearCaja(vendedor, garita, puestoDeTrabajoId);
                }
            }
            return vendedor;
        }

        private void ObtenerOCrearCaja(CredencialMercadoPago vendedor, string garita, int puestoDeTrabajoId)
        {
            var caja = Get<ListaCajaDto>(urlCaja + vendedor.AccessToken).ListaDeCajas.FirstOrDefault(x => x.IdDeReferencia == puestoDeTrabajoId.ToString()) ??
                Post<CajaDto>(urlCaja + vendedor.AccessToken, new CajaRequestDto
                {
                    Nombre = "GaritaDeSalida" + garita,
                    IdDeReferencia = puestoDeTrabajoId.ToString()
                });
            var garitaObj = repositorio.Agregar(new GaritaDeSalida
            {
                CajaId = caja.CajaId,
                CredencialMercadoPago = vendedor,
                PuestoDeTrabajo = repositorio.Obtener<PuestoDeTrabajo>(puestoDeTrabajoId)
            });
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                log.Error(e, "Error al guardar nueva sucursal las credenciales");
            }
        }

        private string RenovarCredencialesVendedor(string accessTokenMarketPlace, string refreshToken)
        {
            var solicitud = new CredencialMercadoPagoDto
            {
                SecretKey = accessTokenMarketPlace,//<====segun la documentacion aca va el token del vendedor pero solo funciona con token del marketplace
                TipoDeSolicitud = "refresh_token",
                TokenActualizarPermiso = refreshToken
            };

            var credenciales = Post<CredencialMercadoPagoDto>(urlSolicitarCredenciales, solicitud);

            return (GuardarCredencial(credenciales)) ?
                "Credenciales Guardadas Exitosamente" : throw new Exception("Hubo un error al Guardar las Credenciales");
        }
        
        private string ObtenerDescripcionDelError(int codigoDeError, string descripcionDeError)
        {
            log.Error($"Error al realizar el cobro en MercadoPago. Codigo:{codigoDeError},Descripcion:{descripcionDeError}");
            var errorObtenido = (codigoDeError == 0) ? descripcionDeError : codigoDeError.ToString();

            if (Enum.TryParse(errorObtenido, true, out CodigoDeErrorMercadoPago error) &&
                Enum.IsDefined(typeof(CodigoDeErrorMercadoPago), error))
            {
                return error.GetType().GetMember(error.ToString())
                                    .First()
                                    .GetCustomAttribute<DisplayAttribute>()
                                    .GetName();
            }
            else if (!string.IsNullOrEmpty(descripcionDeError))
            {
                return $"Error Desconocido al ejecutar la consulta ({descripcionDeError}).";
            }
            else
            {
                return $"Error Desconocido al ejecutar la consulta ({errorObtenido}).";
            }
        }

        public bool GuardarCredencial(CredencialMercadoPagoDto dtoCredencial)
        {
            var IdDelModificado = dtoCredencial.UsuarioVendedorId ?? dtoCredencial.AppId;
            var vendedorOMarketPlace = repositorio.Listar<CredencialMercadoPago>(c => c.UsuarioVendedorId == IdDelModificado || c.AppId == IdDelModificado).FirstOrDefault();

            if(vendedorOMarketPlace == null)
            {
                vendedorOMarketPlace = repositorio.Agregar(new CredencialMercadoPago());
            }

            //generales
            vendedorOMarketPlace.AccessToken = dtoCredencial.AccessToken;
            vendedorOMarketPlace.PublicKey = dtoCredencial.PublicKey;

            //deMarketPlace
            vendedorOMarketPlace.AppId = dtoCredencial.AppId;
            vendedorOMarketPlace.SecretKey = dtoCredencial.SecretKey;
            vendedorOMarketPlace.RedirectUri = dtoCredencial.RedirectUri;

            //deUsuarioVendedor
            vendedorOMarketPlace.FechaVencimientoPermisos = (dtoCredencial.TiempoVencimientoPermiso != 0) ?
            DateTime.Now.AddSeconds(dtoCredencial.TiempoVencimientoPermiso) : (DateTime?)null;
            vendedorOMarketPlace.TokenActualizarPermiso = dtoCredencial.TokenActualizarPermiso;
            vendedorOMarketPlace.UsuarioVendedorId = dtoCredencial.UsuarioVendedorId;
            try
            {
                repositorio.GuardarCambios();
                return true;
            }
            catch (Exception e)
            {
                log.Error(e,"Error al guardar las credenciales");
                return false;
            }
        }

        private T Get<T>(string url)
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var respuestaMercadoPago = httpClient.GetAsync(url).Result;

                if (respuestaMercadoPago.StatusCode == HttpStatusCode.Created ||
                    respuestaMercadoPago.StatusCode == HttpStatusCode.OK ||
                    respuestaMercadoPago.StatusCode == HttpStatusCode.Accepted)
                {
                    return JObject.Parse(respuestaMercadoPago.Content.ReadAsStringAsync().Result)
                         .ToObject<T>();
                }
                else
                {
                    var error = JObject.Parse(respuestaMercadoPago.Content.ReadAsStringAsync().Result)
                                .ToObject<ErrorMercadoPagoDto>().Mensaje;
                    throw new Exception($"{url}: {error}");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Error de conexión en la Solicitud {url}: {e.Message}", e);
            }
            catch (ArgumentNullException e)
            {
                throw new Exception($"{url} enviada sin Argumentos/contenido: {e.Message}", e);
            }
        }

        private T Post<T>(string url, object request, string idempotencia = null)
        {
            try
            {
                var contenido = request != null ? new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json") : null;
                if(idempotencia != null && contenido !=null)
                {
                    contenido.Headers.Add("X-Idempotency-Key", idempotencia);
                }
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var respuestaMercadoPago = httpClient.PostAsync(url, contenido).Result;

                if (respuestaMercadoPago.StatusCode == HttpStatusCode.Created ||
                    respuestaMercadoPago.StatusCode == HttpStatusCode.OK ||
                    respuestaMercadoPago.StatusCode == HttpStatusCode.Accepted)
                {
                    var respuestajson = respuestaMercadoPago.Content.ReadAsStringAsync().Result;
                    log.Debug($"ok MP: {idempotencia}" + respuestajson);
                    return JObject.Parse(respuestaMercadoPago.Content.ReadAsStringAsync().Result)
                         .ToObject<T>();
                }
                else
                {
                    var respuestajson = respuestaMercadoPago.Content.ReadAsStringAsync().Result;
                    log.Error($"Error MP: {idempotencia}" + respuestajson);
                    var respuestaError = JObject.Parse(respuestajson)
                        .ToObject<ErrorMercadoPagoDto>();
                    if(respuestaError.CausaDeError != null)
                    {
                        var erroresMp = respuestaError.CausaDeError.Select(x => new { x.Codigo, x.Descripcion });
                        var todosLosErrores = string.Empty;
                        foreach (var errorCodigo in erroresMp)
                        {
                            var descripcion = ObtenerDescripcionDelError(errorCodigo.Codigo, errorCodigo.Descripcion);
                            todosLosErrores = $"{todosLosErrores}  {descripcion}";
                        }
                        throw new MercadoLibreException($"{todosLosErrores}");
                    }
                    throw new MercadoLibreException($"{url}: {respuestaError.Mensaje}");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Error de conexión en la Solicitud {url}: {e.Message}", e);
            }
            catch (ArgumentNullException e)
            {
                throw new Exception($"{url} enviada sin Argumentos/contenido: {e.Message}", e);
            }
        }

    }
}
