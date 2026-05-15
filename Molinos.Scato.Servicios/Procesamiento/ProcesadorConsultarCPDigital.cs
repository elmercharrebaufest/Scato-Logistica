using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject;
using Ninject.Extensions.Logging;
using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarCPDigital : ProcesadorComando<ConsultarCPDigital>
    {
        private readonly CpePortType serviceAfipCPDigital;
        private readonly IAccesoWsCtg accesoWsCtg;
        private readonly IServicioComandos servicioComandos;
        private IKernel kernel;
        private readonly List<string> erroresNobloqueantes = new List<string>() { Constantes.AFIPCodigoDeError.ErrorPDFNoGenerado };

        public ProcesadorConsultarCPDigital(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos,
                                 CpePortType serviceAfipCPDigital, IAccesoWsCtg accesoWsCtg, IKernel kernel)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.accesoWsCtg = accesoWsCtg;
            this.kernel = kernel;
            this.serviceAfipCPDigital = serviceAfipCPDigital;

            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Ejecuta la consulta de Carta de Porte Electrónica según los parámetros proporcionados.
        /// Soporta búsqueda por patente, CTG directo en AFIP, o con caché como fallback.
        /// </summary>
        /// <exception cref="ArgumentException">Si no se proporcionan parámetros de búsqueda válidos</exception>
        public override Resultado Ejecutar(ConsultarCPDigital comando)
        {
            try
            {
                if (string.IsNullOrEmpty(comando.Patente) && !comando.NroCtg.HasValue)
                {
                    Log.Debug($"ConsultarCPDigital: Sin parámetros de búsqueda. Patente={comando.Patente}, NroCtg={comando.NroCtg}");
                    return new ResultadoCartaPorteElectronica
                    {
                        Errores = { { nameof(comando.NroCtg), "Debe proporcionar una Patente o un número de CTG" } }
                    };
                }

                var tipoBusqueda = DeterminarTipoDeBusqueda(comando);
                Log.Debug($"ConsultarCPDigital: Tipo={tipoBusqueda}, Patente={comando.Patente}, NroCtg={comando.NroCtg}, Centro={comando.CentroId}");

                switch (tipoBusqueda)
                {
                    case TipoBusquedaCPE.BusquedaPorPatente:
                        return BuscarCPEPorPatenteEnCache(comando.Patente, comando.MaterialId, comando.CentroId);

                    case TipoBusquedaCPE.ConsultaDirectaAfip:
                        return BuscarCPEPorCTGEnAFIP(comando.NroCtg.Value, comando.TipoVehiculo, comando.CentroId, comando.ConsultaFerroviarioPorCtg);

                    case TipoBusquedaCPE.CacheConFallbackAfip:
                        var resultadoCache = BuscarCPEPorCTGEnCache(comando.NroCtg.Value, comando.CentroId);
                        if (resultadoCache != null)
                        {
                            Log.Debug($"ConsultarCPDigital: CPE encontrada en caché. CTG={comando.NroCtg}");
                            return resultadoCache;
                        }
                        Log.Debug($"ConsultarCPDigital: CPE no encontrada en caché, consultando AFIP. CTG={comando.NroCtg}");
                        return BuscarCPEPorCTGEnAFIP(comando.NroCtg.Value, comando.TipoVehiculo, comando.CentroId, comando.ConsultaFerroviarioPorCtg);

                    default:
                        Log.Warn($"ConsultarCPDigital: Tipo de búsqueda no reconocido: {tipoBusqueda}");
                        return new ResultadoCartaPorteElectronica
                        {
                            Errores = { { string.Empty, "Tipo de búsqueda no válido" } }
                        };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error en ConsultarCPDigital - Patente={comando.Patente}, NroCtg={comando.NroCtg}, Centro={comando.CentroId}");
                return new ResultadoCartaPorteElectronica 
                { 
                    Errores = { { string.Empty, Textos.Error_Generico } } 
                };
            }
        }

        /// <summary>
        /// Determina el tipo de búsqueda según los parámetros del comando.
        /// </summary>
        /// <remarks>
        /// Precondición: Al menos uno de los campos Patente o NroCtg debe tener valor.
        /// </remarks>
        private TipoBusquedaCPE DeterminarTipoDeBusqueda(ConsultarCPDigital comando)
        {
            if (!string.IsNullOrEmpty(comando.Patente))
                return TipoBusquedaCPE.BusquedaPorPatente;

            return comando.ForzarConsultaAfip 
                ? TipoBusquedaCPE.ConsultaDirectaAfip 
                : TipoBusquedaCPE.CacheConFallbackAfip;
        }

        private ResultadoCartaPorteElectronica BuscarCPEPorPatenteEnCache(string patente, int? materialId, int centroId)
        {
            var resultado = new ResultadoCartaPorteElectronica();

            int diasLimite = ObtenerConfiguracionInt(
                Constantes.ConfiguracionGeneral.Pantalla.LimpiarCacheCartaPorte,
                Constantes.ConfiguracionGeneral.CartaPorteElectronica.DiasLimiteDeBusqueda
            );

            DateTime fecha = DateTime.Today.AddDays(-diasLimite);

            var cpes = Repositorio.Listar<CartaPorteElectronica>(x => 
                x.NroCTG.HasValue && 
                x.Material.HasValue && 
                x.Dominio.StartsWith(patente) &&
                x.FechaEmision.HasValue && x.FechaEmision.Value >= fecha);

            if (materialId.HasValue)
            {
                var material = Repositorio.Obtener<Material>(materialId);
                cpes = cpes.Where(x => x.Material == material.CodigoEspecie).ToList();
            }

            if (!cpes.Any())
            {
                resultado.Errores.Add(nameof(ConsultarCPDigital.NroCtg), "Debe ingresar el número de CTG");
                return resultado;
            }

            var cpe = cpes.OrderByDescending(c => c.FechaEmision).FirstOrDefault();
            resultado.PdfImage = ConvertirPDFenPNG(cpe.Pdf);
            resultado.Pdf = cpe.Pdf;
            var cpeDto = ConvertirCartaPorteDto(cpe, centroId, resultado);
            resultado.Cpe = cpeDto;

            return resultado;
        }

        private ResultadoCartaPorteElectronica BuscarCPEPorCTGEnCache(long ctg, int centroId)
        {
            var resultado = new ResultadoCartaPorteElectronica();

            var cpe = Repositorio.ObtenerMasReciente<CartaPorteElectronica>(x => x.NroCTG == ctg, x => x.FechaEmision.Value);
            if (cpe is null)
                return null;

            resultado.PdfImage = ConvertirPDFenPNG(cpe.Pdf);
            var cpeDto = ConvertirCartaPorteDto(cpe, centroId, resultado);
            resultado.Cpe = cpeDto;

            return resultado;
        }

        private ResultadoCartaPorteElectronica BuscarCPEPorCTGEnAFIP(long ctg, int tipoVehiculo, int centroId, bool consultaFerroviarioPorCtg)
        {
            var resultado = new ResultadoCartaPorteElectronica();
            var centro = Repositorio.Obtener<Centro>(centroId);
            var auth = accesoWsCtg.ObtenerAuth(centro.Cuit.Replace("-", string.Empty), resultado);
            if (resultado.HayErrores) return resultado;

            if (tipoVehiculo == (int)TipoVehiculo.Tren)
                return BuscarCPEPorCTGEnAFIPFerroviaria(ctg, auth, consultaFerroviarioPorCtg, centroId);

            return BuscarCPEPorCTGEnAFIPAutomotor(ctg, auth, centroId);
        }

        private ResultadoCartaPorteElectronica BuscarCPEPorCTGEnAFIPFerroviaria(long ctg, Auth auth, bool consultaFerroviarioPorCtg, int centroId)
        {
            if (consultaFerroviarioPorCtg)
                return BuscarCPEPorCTGEnAFIPFerroviariaPorCtg(ctg, auth, centroId);
            else
                return BuscarCPEPorCTGEnAFIPFerroviariaPorOperativo(ctg, auth, centroId);
        }

        private ResultadoCartaPorteElectronica BuscarCPEPorCTGEnAFIPFerroviariaPorCtg(long nroCTG, Auth auth, int centroId)
        {
            var resultado = new ResultadoCartaPorteElectronica();
            var request = CrearRequestFerroviariaPorCtg(nroCTG, auth);
            Log.Debug(request.ToXml());

            var responseCp = serviceAfipCPDigital.consultarCPEFerroviaria(request);
            if (ProcesarErroresAfip(responseCp.respuesta, resultado))
                return resultado;

            ProcesarRespuestaExitosaAFIP(responseCp.respuesta, centroId, resultado);
            return resultado;
        }

        private ResultadoCartaPorteElectronica BuscarCPEPorCTGEnAFIPFerroviariaPorOperativo(long nroOperativo, Auth auth, int centroId)
        {
            var resultado = new ResultadoCartaPorteElectronica();
            var requestPorOperativo = CrearRequestFerroviariaPorOperativo(nroOperativo, auth);

            var responseCp = serviceAfipCPDigital.consultaCPEFerroviariaPorNroOperativo(requestPorOperativo);
            if (ProcesarErroresAfip(responseCp.respuesta, resultado))
                return resultado;

            var listaVagones = ProcesarVagonesFerroviarios(responseCp.respuesta, auth, centroId, resultado);
            if (!resultado.HayErrores)
                resultado.Cpe.Vehiculos = listaVagones;

            return resultado;
        }

        private ResultadoCartaPorteElectronica BuscarCPEPorCTGEnAFIPAutomotor(long nroCTG, Auth auth, int centroId)
        {
            var resultado = new ResultadoCartaPorteElectronica();
            var request = CrearRequestAutomotor(nroCTG, auth);
            Log.Debug(request.ToXml());

            var responseCp = serviceAfipCPDigital.consultarCPEAutomotor(request);
            if (ProcesarErroresAfip(responseCp.respuesta, resultado))
                return resultado;

            ProcesarRespuestaExitosaAFIP(responseCp.respuesta, centroId, resultado);
            return resultado;
        }

        #region Métodos de Creación de Requests AFIP

        private consultarCPEFerroviariaRequest CrearRequestFerroviariaPorCtg(long nroCTG, Auth auth)
        {
            return new consultarCPEFerroviariaRequest
            {
                auth = auth,
                solicitud = new ConsultarFerroviariaSolicitud
                {
                    nroCTG = nroCTG,
                    nroCTGSpecified = true
                }
            };
        }

        private consultaCPEFerroviariaPorNroOperativoRequest CrearRequestFerroviariaPorOperativo(long nroOperativo, Auth auth)
        {
            return new consultaCPEFerroviariaPorNroOperativoRequest
            {
                auth = auth,
                solicitud = new ConsultaCPEFerroviariaPorNroOperativoSolicitud
                {
                    nroOperativo = nroOperativo
                }
            };
        }

        private consultarCPEAutomotorRequest CrearRequestAutomotor(long nroCTG, Auth auth)
        {
            return new consultarCPEAutomotorRequest
            {
                auth = auth,
                solicitud = new ConsultarAutomotorSolicitud
                {
                    nroCTG = nroCTG,
                    nroCTGSpecified = true
                }
            };
        }

        #endregion

        #region Métodos de Procesamiento de Respuestas AFIP

        private bool ProcesarErroresAfip(dynamic respuesta, ResultadoCartaPorteElectronica resultado)
        {
            if (respuesta == null)
            {
                resultado.Error(Constantes.AFIPCodigoDeError.NoExistenSolicitudes, "No se obtuvo respuesta desde AFIP");
                return true;
            }

            if (respuesta.errores == null || !((IEnumerable<dynamic>)respuesta.errores).Any())
                return false;

            var erroresNobloqueanteSet = new HashSet<string>(erroresNobloqueantes);
            var errores = (IEnumerable<dynamic>)respuesta.errores;
            var tieneErrorBloqueante = errores.Any(e => !erroresNobloqueanteSet.Contains((string)e.codigo));

            if (tieneErrorBloqueante)
            {
                var errorNoExiste = errores.Any(e => (string)e.codigo == Constantes.AFIPCodigoDeError.NoExistenSolicitudes);
                var mensaje = errorNoExiste ? "No se encuentran datos" : (string)errores.FirstOrDefault()?.descripcion;
                resultado.Error(Constantes.AFIPCodigoDeError.NoExistenSolicitudes, mensaje);
            }

            foreach (var error in errores)
                Log.Error($"ProcesadorConsultarCPDigital - ({error.codigo}) {error.descripcion}");

            return tieneErrorBloqueante;
        }

        private void ProcesarRespuestaExitosaAFIP(dynamic respuestaAFIP, int centroId, ResultadoCartaPorteElectronica resultado)
        {
            var cartaPorteEntity = ConvertirResponseAFIPenCartaPorteElectronica(respuestaAFIP);
            if (cartaPorteEntity == null)
                return;

            RegistrarCartaPorteElectronica(cartaPorteEntity);
            resultado.PdfImage = ConvertirPDFenPNG(respuestaAFIP.pdf);
            resultado.Pdf = respuestaAFIP.pdf;
            resultado.Cpe = ConvertirCartaPorteDto(cartaPorteEntity, centroId, resultado);
        }

        private List<VehiculoDto> ProcesarVagonesFerroviarios(dynamic respuestaAFIP, Auth auth, int centroId, ResultadoCartaPorteElectronica resultado)
        {
            var listaVagones = new List<VehiculoDto>();
            var cartasPorte = respuestaAFIP?.cartaPorte as IEnumerable<dynamic>;
            resultado.CTGsDeOperativo = cartasPorte?.Select(cp => (long)cp.nroCTG).ToList();

            if (resultado.CTGsDeOperativo == null)
                return listaVagones;

            foreach (var ctgFerroviaria in resultado.CTGsDeOperativo)
            {
                var resultadoCpe = BuscarCPEPorCTGEnAFIPFerroviariaPorCtg(ctgFerroviaria, auth, centroId);
                if (resultadoCpe.HayErrores)
                    break;

                var vehiculo = resultadoCpe.Cpe?.Vehiculos?.FirstOrDefault();
                if (vehiculo != null)
                {
                    EnriquecerVehiculoFerroviario(vehiculo, resultadoCpe.Cpe);
                    listaVagones.Add(vehiculo);
                }
            }

            return listaVagones;
        }

        private void EnriquecerVehiculoFerroviario(VehiculoDto vehiculo, CartaPorteDto cpe)
        {
            vehiculo.NumCTG = cpe?.NroCartaPorte;
            vehiculo.Sucural = cpe?.Sucursal?.ToString("D5");
            vehiculo.NumOrden = cpe?.NroOrden?.ToString("D8");
            vehiculo.TipoVehiculo = TipoVehiculo.Tren;
        }

        #endregion

        private CartaPorteDto ConvertirCartaPorteDto(CartaPorteElectronica cartaPorte, int centroId, Resultado resultado)
        {
            var centro = Repositorio.Obtener<Centro>(centroId);
            var categoriaStr = cartaPorte.NroCTG.ToString().Substring(0, 3).EndsWith("01") ? "PRODUCTOR" : "OPERADOR";
            var categoria = Repositorio.Obtener<Categoria>(x => x.Clasificacion == categoriaStr);

            var cpeCuitOrigen = cartaPorte.CuitOrigen.ToString();
            var titular = ObtenerProveedor(cpeCuitOrigen, resultado, Textos.CartaPorte_TitularCartaPorte, false, false, true);
            var intermediario = ObtenerProveedor(cartaPorte.CuitIntermediario.ToString(), resultado, Textos.CartaPorte_Intermediario, false, false, true);
            var rtteComercialProductor = ObtenerProveedor(cartaPorte.CuitRemitenteComercialProductor.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true);
            var rtteComercial = ObtenerProveedor(cartaPorte.CuitRemitenteComercialVentaPrimaria.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true);
            var rtteComercialVentaSecundaria = ObtenerProveedor(cartaPorte.CuitRemitenteComercialVentaSecundaria.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true);
            var rtteComercialVentaSecundaria2 = ObtenerProveedor(cartaPorte?.CuitRemitenteComercialVentaSecundaria2.ToString(), resultado, Textos.Rtte_comercial_venta_secundaria_2, false, false, true);
            var corredor = ObtenerProveedor(cartaPorte?.CuitCorredorVentaPrimaria?.ToString(), resultado, Textos.Corredor_primario, false, true, false);
            var corredorVendedorSecundario = ObtenerProveedor(cartaPorte.CuitCorredorVentaSecundaria.ToString(), resultado, Textos.CartaPorte_CorredorVendedor, false, true, false);
            var agenteCompras = ObtenerProveedor(cartaPorte.CuitMercadoATermino.ToString(), resultado, Textos.CartaPorte_AgenteCompras, false, false, true);
            var cpeCuitRepresentanteEntregador = cartaPorte.CuitRepresentanteEntregador.ToString();
            var entregador = Repositorio.Listar<Entregador>(x => x.Cuil.Replace("-", "") == cpeCuitRepresentanteEntregador && x.Activo).LastOrDefault() ?? Repositorio.Listar<Entregador>(x => x.RazonSocial.ToUpper().Contains("SIN ENTREGA")).LastOrDefault();
            var corredorVendedor = ObtenerProveedor(cartaPorte.CuitCorredorVentaPrimaria.ToString(), resultado, Textos.CartaPorte_CorredorVendedor, false, true, false);
            var cpeCuitRepresentanteRecibidor = cartaPorte.CuitRepresentanteRecibidor.ToString();
            var representanteRecibidor = Repositorio.Listar<Entregador>(x => x.Cuil.Replace("-", "") == cpeCuitRepresentanteRecibidor && x.Activo).LastOrDefault();
            var destinatario = ObtenerProveedor(cartaPorte.CuitDestinatario.ToString(), resultado, Textos.CartaPorte_Destinatario, false, false, true);
            var intermediarioFlete = ObtenerProveedor(cartaPorte.CuitIntermediarioFlete.ToString(), resultado, Textos.CartaPorte_Intermediario, false, false, true);
            var pagadorFlete = ObtenerProveedor(cartaPorte.CuitPagadorFlete.ToString(), resultado, Textos.CartaPorte_Transportista_Pagador_Flete, false, false, true);
            var transportista = ObtenerTransportista(cartaPorte.CuitTransportista.ToString(), resultado);

            var material = Repositorio.Obtener<Material>(x => x.CodigoEspecie == cartaPorte.Material && x.Activo);
            var cosecha = cartaPorte.Cosecha.HasValue ? cartaPorte.Cosecha.Value.ToString() : string.Empty;
            var cpeLocalidad = cartaPorte.Localidad.Value.ToString();
            var localidad = Repositorio.Listar<Localidad>(x => x.CodigoAfip == cpeLocalidad).FirstOrDefault() ?? ObtenerLocalidadAfip(centro.Cuit, cpeLocalidad, cartaPorte.Provincia.Value);
            var codEstab = cartaPorte.PlantaOrigen.HasValue ? cartaPorte.PlantaOrigen.ToString() : string.Empty;

            var cpeCuitchofer = cartaPorte.CuitChofer.ToString();
            var chofer = Repositorio.Obtener<Chofer>(x => x.Cuil.Replace("-", "") == cpeCuitchofer);
            var patentes = cartaPorte.Dominio.Split(',');
            var patente = patentes.FirstOrDefault();
            var acoplado = patentes.Length > 1 ? patentes.LastOrDefault() : string.Empty;
            var vehiculos = new List<Vehiculo>
            {
                new Vehiculo
                {
                    Patente = patente,
                    PatenteAcoplado = acoplado,
                    PesoBrutoOrigen = cartaPorte.PesoBruto.GetValueOrDefault(),
                    PesoTaraOrigen = cartaPorte.PesoTara.GetValueOrDefault(),
                    PesoNetoOrigen = (cartaPorte.PesoBruto.GetValueOrDefault() - cartaPorte.PesoTara.GetValueOrDefault())
                }
            };

            var entidad = new Dominio.Entidades.CartaPorte
            {
                Id = cartaPorte.Id,
                Cpe = true,
                NroCartaPorte = cartaPorte.NroCTG.ToString(),
                FechaCP = cartaPorte.FechaCP.Value,
                FechaEmision = cartaPorte.FechaEmision.Value,
                FechaVto = cartaPorte.FechaVto.Value,
                Categoria = categoria,
                Sucursal = cartaPorte.Sucursal,
                Observacion = cartaPorte.Observacion,

                //Traslado
                TitularCartaPorte = titular,
                Intermediario = intermediario,
                RtteComercialProductor = rtteComercialProductor,
                RtteComercial = rtteComercial,
                RtteComercialVentaSecundaria = rtteComercialVentaSecundaria,
                RtteComercialVentaSecundaria2 = rtteComercialVentaSecundaria2,
                Corredor = corredor,
                CorredorVendedorSecundario = corredorVendedorSecundario,
                AgenteCompras = agenteCompras,
                Entregador = entregador,
                CorredorVendedor = corredorVendedor,
                RepresentanteRecibidor = representanteRecibidor,
                Destinatario = destinatario,
                CentroDestino = centro,
                IntermediarioFlete = intermediarioFlete,
                PagadorFlete = pagadorFlete,
                Transportista = transportista,

                // Grano
                Material = material,
                Cosecha = cosecha,
                Procedencia = localidad,
                CodEstab = codEstab,
                Cupo = cartaPorte.CodigoTurno,

                //Transporte
                Chofer = chofer,
                Vehiculos = vehiculos,
                KmRecorrer = cartaPorte.KmRecorrer,
                TarifaTonelada = (decimal)cartaPorte.Tarifa,
                TarifaReferencia = (decimal)cartaPorte.TarifaReferencia,
            };

            var cpe = Conversor.Convertir<Dominio.Entidades.CartaPorte, CartaPorteDto>(entidad);
            cpe.NroOrden = cartaPorte.NroOrden;
            cpe.Destino = centro.Descripcion;
            cpe.DestinoId = centro.Id;
            cpe.TipoVehiculo = TipoVehiculo.Camión;
            cpe.EstadoCpe = cartaPorte.Estado;
            cpe.EsTransportista = transportista != null;
            cpe.CodigoRENSPA = VisecHelper.ObtenerTipoOrigenCPE(cartaPorte.PlantaOrigen.GetValueOrDefault()) == TipoOrigenCPE.UnidadProductiva
                               ? (!string.IsNullOrWhiteSpace(cartaPorte.NroRenspa)
                                    ? cartaPorte.NroRenspa
                                    : VisecHelper.ObtenerRENSPA(cpe.Observacion))
                               : string.Empty;

            return cpe;
        }

        private Localidad ObtenerLocalidadAfip(string centroCuit, string localidadAfip, int provinciaAfip)
        {
            var auth = accesoWsCtg.ObtenerAuth(centroCuit.Replace("-", string.Empty), new Resultado());
            Localidad localidad = null;
            var request = new consultarLocalidadesPorProvinciaRequest()
            {
                auth = auth,
                solicitud = new ConsultarLocalidadesPorProvinciaSolicitud()
                {
                    codProvincia = provinciaAfip
                }
            };
            var response = serviceAfipCPDigital.consultarLocalidadesPorProvincia(request);
            if (response == null) return null;
            if (response.respuesta != null && response.respuesta.errores != null && response.respuesta.errores.Any()) return null;

            var loc = response.respuesta.localidad.FirstOrDefault(x => x.codigo == localidadAfip);
            if (loc != null)
            {
                var provincia = Repositorio.Listar<Provincia>(x => x.CodigoAfip == provinciaAfip).FirstOrDefault();
                localidad = new Localidad
                {
                    Provincia = provincia,
                    CodigoAfip = loc.codigo,
                    Descripcion = loc.descripcion.Length > 50 ? loc.descripcion.Substring(0, 50) : loc.descripcion
                };
                Repositorio.Agregar(localidad);
                Repositorio.GuardarCambios();
            }
            return localidad;
        }

        private Proveedor ObtenerProveedor(string cuitSinGuiones, Resultado resultado, string nombreDato, bool am, bool cm, bool pr)
        {
            if (!string.IsNullOrEmpty(cuitSinGuiones) && cuitSinGuiones.Trim() == "0") return null;

            var cuil = FormatterHelper.ConvertirCuilConGuionesSinException(cuitSinGuiones);
            if (cuil == null) return null;

            var proveedor = Repositorio.Listar<Proveedor>(x => x.Cuil == cuil && ((x.AM == am && am) || (x.CM == cm && cm) || (x.PR == pr && pr)) && x.Activo).LastOrDefault();
            if (proveedor == null)
            {
                servicioComandos.Ejecutar(new SincronizarProveedores { RetornarResultado = false, Cuit = cuil, CargaMasiva = false });
                proveedor = Repositorio.Listar<Proveedor>(x => x.Cuil == cuil && ((x.AM == am && am) || (x.CM == cm && cm) || (x.PR == pr && pr)) && x.Activo).LastOrDefault();
            }

            if (proveedor == null)
                resultado.Error(nombreDato, string.Format(Textos.Proveedor_NoEncontrado, nombreDato, cuil));

            return proveedor;
        }

        private Transportista ObtenerTransportista(string cuitTransportista, Resultado resultado)
        {
            var cuit = FormatterHelper.ConvertirCuilConGuionesSinException(cuitTransportista);
            if (cuit == null)
            {
                resultado.Error(nameof(CartaPorteDto.Transportista), Textos.CuitInvalido);
                return null;
            }

            var transportista = Repositorio.Listar<Transportista>(x => x.Cuit == cuit).LastOrDefault();
            if (transportista == null)
                resultado.Error(nameof(CartaPorteDto.Transportista), string.Format(Textos.Transportista_NoEncontrado, cuitTransportista));

            return transportista;
        }

        private CartaPorteElectronica ConvertirResponseAFIPenCartaPorteElectronica(dynamic responseAFIP)
        {
            if (responseAFIP == null) return null;

            try
            {
                var cabeceraAFIP = responseAFIP.cabecera;
                var origenAFIP = responseAFIP.origen;
                var retiroProductorAFIP = responseAFIP.retiroProductor;
                var intervinientesAFIP = responseAFIP.intervinientes;
                var datosCargaAFIP = responseAFIP.datosCarga;
                var destinoAFIP = responseAFIP.destino;
                var destinatarioAFIP = responseAFIP.destinatario;
                var transporteAFIP = responseAFIP.transporte;

                string dominio = string.Empty;
                if (transporteAFIP != null && transporteAFIP.dominio != null && transporteAFIP.dominio.Length > 0)
                    dominio = string.Join(",", transporteAFIP.dominio);

                var carta = new CartaPorteElectronica
                {
                    // cabecera
                    TipoCartaPorte = cabeceraAFIP != null ? cabeceraAFIP.tipoCartaPorte : (int?)null,
                    Sucursal = cabeceraAFIP != null ? cabeceraAFIP.sucursal : (int?)null,
                    NroOrden = cabeceraAFIP != null ? cabeceraAFIP.nroOrden : (long?)null,
                    NroCTG = cabeceraAFIP != null ? cabeceraAFIP.nroCTG : (long?)null,
                    FechaEmision = cabeceraAFIP != null ? cabeceraAFIP.fechaEmision : (DateTime?)null,
                    Estado = cabeceraAFIP != null ? cabeceraAFIP.estado : null,
                    FechaCP = cabeceraAFIP != null ? cabeceraAFIP.fechaInicioEstado : (DateTime?)null,
                    FechaVto = cabeceraAFIP != null ? cabeceraAFIP.fechaVencimiento : (DateTime?)null,
                    Observacion = cabeceraAFIP != null ? cabeceraAFIP.observaciones : null,

                    // origen
                    Provincia = origenAFIP != null ? origenAFIP.codProvincia : (int?)null,
                    Localidad = origenAFIP != null ? origenAFIP.codLocalidad : (int?)null,
                    Domicilio = origenAFIP != null ? origenAFIP.domicilio : null,
                    PlantaOrigen = origenAFIP != null ? origenAFIP.planta : (int?)null,
                    CuitOrigen = origenAFIP != null ? origenAFIP.cuit : (long?)null,
                    NroRenspa = origenAFIP != null ? origenAFIP.nroRenspa : null,

                    // correspondeRetiroProductor
                    RetiroProductor = responseAFIP != null ? responseAFIP.correspondeRetiroProductor : (bool?)false,

                    // retiroProductor
                    CuitRemitenteComercialProductor = retiroProductorAFIP != null ? retiroProductorAFIP.cuitRemitenteComercialProductor : (long?)null,

                    // intervinientes
                    CuitRemitenteComercialVentaPrimaria = intervinientesAFIP != null ? intervinientesAFIP.cuitRemitenteComercialVentaPrimaria : (long?)null,
                    CuitRemitenteComercialVentaSecundaria = intervinientesAFIP != null ? intervinientesAFIP.cuitRemitenteComercialVentaSecundaria : (long?)null,
                    CuitIntermediario = retiroProductorAFIP != null ? retiroProductorAFIP.cuitRemitenteComercialProductor : (long?)0,
                    CuitMercadoATermino = intervinientesAFIP != null ? intervinientesAFIP.cuitMercadoATermino : (long?)null,
                    CuitCorredorVentaPrimaria = intervinientesAFIP != null ? intervinientesAFIP.cuitCorredorVentaPrimaria : (long?)null,
                    CuitCorredorVentaSecundaria = intervinientesAFIP != null ? intervinientesAFIP.cuitCorredorVentaSecundaria : (long?)null,
                    CuitRepresentanteEntregador = intervinientesAFIP != null ? intervinientesAFIP.cuitRepresentanteEntregador : (long?)null,
                    CuitRepresentanteRecibidor = intervinientesAFIP != null ? intervinientesAFIP.cuitRepresentanteRecibidor : (long?)null,
                    CuitRemitenteComercialVentaSecundaria2 = intervinientesAFIP != null ? intervinientesAFIP.cuitRemitenteComercialVentaSecundaria2 : (long?)null,

                    // datosCarga
                    Material = datosCargaAFIP != null ? datosCargaAFIP.codGrano : null,
                    PesoBruto = datosCargaAFIP != null ? datosCargaAFIP.pesoBruto : (double?)null,
                    PesoTara = datosCargaAFIP != null ? datosCargaAFIP.pesoTara : (double?)null,
                    Cosecha = datosCargaAFIP != null ? datosCargaAFIP.cosecha : (int?)null,

                    // destino
                    CuitDestino = destinoAFIP != null ? destinoAFIP.cuit : (long?)null,
                    LocalidadDestino = destinoAFIP != null ? destinoAFIP.codLocalidad : (int?)null,
                    ProvinciaDestino = destinoAFIP != null ? destinoAFIP.codProvincia : (int?)null,
                    PlantaDestino = destinoAFIP != null ? destinoAFIP.planta : (int?)null,

                    // destinatario
                    CuitDestinatario = destinatarioAFIP != null ? destinatarioAFIP.cuit : (long?)null,

                    // transporte
                    CuitTransportista = transporteAFIP != null ? transporteAFIP.cuitTransportista : (long?)null,
                    Dominio = dominio,
                    FechaPartida = transporteAFIP != null ? transporteAFIP.fechaHoraPartida : (DateTime?)null,
                    KmRecorrer = transporteAFIP != null ? transporteAFIP.kmRecorrer : (int?)null,
                    CodigoTurno = transporteAFIP != null ? transporteAFIP.codigoTurno : null,

                    CuitChofer = transporteAFIP != null ? transporteAFIP.cuitChofer : (long?)null,
                    Tarifa = transporteAFIP != null && transporteAFIP.tarifa != null ? Convert.ToDouble(transporteAFIP.tarifa) : 0,
                    CuitPagadorFlete = transporteAFIP != null ? transporteAFIP.cuitPagadorFlete : (long?)null,
                    CuitIntermediarioFlete = transporteAFIP != null ? transporteAFIP.cuitIntermediarioFlete : (long?)null,
                    MercaderiaFumigada = transporteAFIP != null ? transporteAFIP.mercaderiaFumigada : (bool?)null,
                    FechaUltimaActualizacion = null,
                    Pdf = responseAFIP != null ? responseAFIP.pdf : null,
                    TarifaReferencia = transporteAFIP != null && transporteAFIP.tarifaReferencia != null ? Convert.ToDouble(transporteAFIP.tarifaReferencia) : 0,
                    FechaCacheado = DateTime.Now
                };

                return carta;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en ConvertirResponseAFIPenCartaPorteElectronica");
                return null;
            }
        }

        private byte[] ConvertirPDFenPNG(byte[] pdf)
        {
            if (pdf is null) return null;

            using (var document = PdfDocument.Load(new MemoryStream(pdf)))
            {
                var dpix = ConfigurationManager.AppSettings["PdfCpeDpiX"];
                var dpiy = ConfigurationManager.AppSettings["PdfCpeDpiY"];

                var image = document.Render(0, string.IsNullOrEmpty(dpix) ? 600 : Convert.ToInt32(dpix), string.IsNullOrEmpty(dpiy) ? 600 : Convert.ToInt32(dpiy), PdfRenderFlags.ForPrinting | PdfRenderFlags.CorrectFromDpi);
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        private void RegistrarCartaPorteElectronica(CartaPorteElectronica cpe)
        {
            var cpeExistente = Repositorio.Obtener<CartaPorteElectronica>(x => x.NroCTG == cpe.NroCTG);

            if (cpeExistente != null)
                ActualizarCartaPorteElectronica(cpeExistente, cpe);
            else
                Repositorio.Agregar(cpe);

            Repositorio.GuardarCambios();
        }

        private void ActualizarCartaPorteElectronica(CartaPorteElectronica destino, CartaPorteElectronica origen)
        {
            var idOriginal = destino.Id;

            Conversor.Convertir(origen, destino);

            destino.Id = idOriginal;
            destino.FechaUltimaActualizacion = DateTime.Now;
            destino.FechaCacheado = DateTime.Now;
        }

        private int ObtenerConfiguracionInt(string pantalla, string nombre)
        {
            var valor = Repositorio.Obtener<ConfiguracionGeneral>(
                x => x.Pantalla == pantalla && x.Nombre == nombre
            )?.Valor;

            if (!int.TryParse(valor, out int resultado))
                throw new InvalidOperationException(
                    $"Configuración inválida: {pantalla} - {nombre}"
                );

            return resultado;
        }
    }
}