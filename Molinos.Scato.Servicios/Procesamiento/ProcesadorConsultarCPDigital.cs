using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
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

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarCPDigital : ProcesadorComando<ConsultarCPDigital>
    {
        const int cLongitudMaximaCupo = 16; //Se establece el limite maximo de caracteres para el campo cupo de la consulta de CTG.
        private readonly CpePortType serviceAfipCPDigital;
        private readonly IAccesoWsCtg accesoWsCtg;
        private IKernel kernel;

        public ProcesadorConsultarCPDigital(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCPDigital, IAccesoWsCtg accesoWsCtg, IKernel kernel)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.kernel = kernel;
            this.serviceAfipCPDigital = serviceAfipCPDigital;
        }

        public override Resultado Ejecutar(ConsultarCPDigital comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new ResultadoCartaPorteElectronica();
            var centro = Repositorio.Obtener<Centro>(comando.CentroId);

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            Log.Debug("ProcesadorConsultarCPDigital - Creo la autorizacion");
            var auth = accesoWsCtg.ObtenerAuth(centro.Cuit.Replace("-", string.Empty), resultado);

            if (!comando.ConsultaAfip && centro.ContingenciaAfipCpe && !comando.ConsultaImagenCpe)
            {
                var cartaPorteElectronica = Repositorio.Listar<CartaPorteElectronica>(x => x.NroCTG == comando.NroCtg).FirstOrDefault();
                if (cartaPorteElectronica != null)
                {
                    cartaPorteElectronica.CuitIntermediario = cartaPorteElectronica.RetiroProductor == true ? cartaPorteElectronica.CuitRemitenteComercialProductor : 0;
                    
                    var cpe = ConvertirCartaPorteDto(auth, cartaPorteElectronica, centro, resultado, comando.ConsultaMinima);
                    resultado.Cpe = cpe;
                    return resultado;
                }
            }

            try
            {
                var cartaPorte = Repositorio.Obtener<CartaPorteElectronica>(x => x.NroCTG == comando.NroCtg);
                var continuar = true;
                var erroresNobloqueantes = new List<string>() { "550" }; //no se pudo generar el pdf

                if (comando?.TipoVehiculo == (int)TipoVehiculo.Tren)
                {
                    if (comando.ConsultaFerroviarioPorCtg)
                    {
                        Log.Debug("ProcesadorConsultarCPDigital - TipoTren - PorCTG");
                        resultado = GuardarCartaPorteElectronica(comando.NroCtg, auth, centro);
                        if(!resultado.HayErrores) {
                            foreach (var itemVehiculo in resultado.Cpe.Vehiculos)
                            {
                                itemVehiculo.NumCTG  = resultado?.Cpe?.NroCartaPorte;
                                itemVehiculo.Sucural = resultado?.Cpe?.Sucursal?.ToString("D5");
                                itemVehiculo.NumOrden = resultado?.Cpe?.NroOrden?.ToString("D8");
                                itemVehiculo.TipoVehiculo = TipoVehiculo.Tren;
                            }
                        }
                    } else
                    {
                        var listaVagones = new List<VehiculoDto>();
                        Log.Debug("ProcesadorConsultarCPDigital - TipoTren - PorOperativo");
                        resultado = ObtenerCTGsPorNroDeOperativo(comando.NroCtg, auth);

                        if (!resultado.HayErrores)
                        {
                            foreach (var CTGFerroviaria in resultado.CTGsDeOperativo)
                            {
                                resultado = GuardarCartaPorteElectronica(CTGFerroviaria, auth, centro);
                                if (resultado.HayErrores)
                                {
                                    break;
                                }
                                else
                                {
                                    var vehiculo = resultado.Cpe.Vehiculos.FirstOrDefault();
                                    if (!(vehiculo is null))
                                    {
                                        vehiculo.NumCTG = resultado?.Cpe?.NroCartaPorte;
                                        vehiculo.Sucural = resultado?.Cpe?.Sucursal?.ToString("D5");
                                        vehiculo.NumOrden = resultado?.Cpe?.NroOrden?.ToString("D8");
                                        vehiculo.TipoVehiculo = TipoVehiculo.Tren;
                                        listaVagones.Add(vehiculo);
                                    }
                                }
                            }
                            if (!resultado.HayErrores)
                            {
                                resultado.Cpe.Vehiculos = listaVagones;                                
                            }
                        }
                    }

                    if (!resultado.HayErrores)
                    {
                        var choferDefault = Repositorio.Listar<Chofer>(x => x.Cuil == "99-99999999-9").FirstOrDefault();
                        if (!(choferDefault is null))
                        {
                            resultado.Cpe.Chofer = Conversor.Convertir<Dominio.Entidades.Chofer, ChoferDto>(choferDefault);
                        }
                    }
                }
                else
                {
                    Log.Debug("ProcesadorConsultarCPDigital - Automotor");
                    // Armo la consulta automotor
                    var request = new consultarCPEAutomotorRequest
                    {
                        auth = auth,
                        solicitud = new ConsultarAutomotorSolicitud
                        {
                            nroCTG = comando.NroCtg,
                            nroCTGSpecified = true
                        }
                    };

                    Log.Debug(request.ToXml());

                    var responseCp = serviceAfipCPDigital.consultarCPEAutomotor(request);

                    if (responseCp.respuesta == null)
                    {
                        resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                        Log.Debug($"Consulta de CPE ctg {comando.NroCtg} sin respuesta");
                        return resultado;
                    }

                    if (responseCp.respuesta != null && responseCp.respuesta.errores != null && responseCp.respuesta.errores.Any())
                    {
                        if (!erroresNobloqueantes.Any(x => responseCp.respuesta.errores.Any(y => y.codigo == x)))
                        {
                            foreach (var error in responseCp.respuesta.errores)
                            {
                                resultado.Errores.Add("2", error.descripcion);
                            }

                            continuar = false;
                        }

                        foreach (var error in responseCp.respuesta.errores)
                        {
                            Log.Error(string.Format("ProcesadorConsultarCPDigital - ({0}) {1}", error.codigo, error.descripcion));
                        }
                    }

                    if (responseCp.respuesta != null && continuar)
                    {
                        if(responseCp?.respuesta?.pdf != null)
                        {
                            try
                            {
                                using (var document = PdfDocument.Load(new MemoryStream(responseCp.respuesta.pdf)))
                                {
                                    var dpix = ConfigurationManager.AppSettings["PdfCpeDpiX"];
                                    var dpiy = ConfigurationManager.AppSettings["PdfCpeDpiY"];

                                    var image = document.Render(0, string.IsNullOrEmpty(dpix) ? 600 : Convert.ToInt32(dpix), string.IsNullOrEmpty(dpiy) ? 600 : Convert.ToInt32(dpiy), PdfRenderFlags.ForPrinting | PdfRenderFlags.CorrectFromDpi);
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        resultado.PdfImage = ms.ToArray();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, $"Error al obtener pdf de ctg {comando.NroCtg}");
                            }
                        }

                        var cartaPorteRequest = new CartaPorteElectronica
                        {
                            //cabecera
                            TipoCartaPorte = responseCp.respuesta.cabecera.tipoCartaPorte,
                            Sucursal = responseCp.respuesta.cabecera.sucursal,
                            NroOrden = responseCp.respuesta.cabecera.nroOrden,
                            NroCTG = responseCp.respuesta.cabecera.nroCTG,
                            FechaEmision = responseCp.respuesta.cabecera.fechaEmision,
                            Estado = responseCp.respuesta.cabecera.estado,
                            FechaCP = responseCp.respuesta.cabecera.fechaInicioEstado,
                            FechaVto = responseCp.respuesta.cabecera.fechaVencimiento,
                            Observacion = responseCp.respuesta.cabecera.observaciones,
                            //origen
                            Provincia = responseCp.respuesta.origen.codProvincia,
                            Localidad = responseCp.respuesta.origen.codLocalidad,
                            Domicilio = responseCp.respuesta.origen.domicilio,
                            PlantaOrigen = responseCp.respuesta.origen.planta,
                            CuitOrigen = responseCp.respuesta.origen.cuit,

                            //correspondeRetiroProductor
                            RetiroProductor = responseCp.respuesta.correspondeRetiroProductor,

                            //retiroProductor

                            //CertificadoCOE = responseCp.respuesta.retiroProductor!= null ? responseCp.respuesta.retiroProductor.certificadoCOE: (long?)null,
                            CuitRemitenteComercialProductor = responseCp.respuesta.retiroProductor != null ? responseCp.respuesta.retiroProductor.cuitRemitenteComercialProductor : (long?)null,

                            //intervinientes
                            CuitRemitenteComercialVentaPrimaria = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaPrimaria,
                            CuitRemitenteComercialVentaSecundaria = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaSecundaria,
                            CuitIntermediario = responseCp?.respuesta?.retiroProductor?.cuitRemitenteComercialProductor ?? 0,
                            CuitMercadoATermino = responseCp.respuesta.intervinientes.cuitMercadoATermino,
                            CuitCorredorVentaPrimaria = responseCp.respuesta.intervinientes.cuitCorredorVentaPrimaria,
                            CuitCorredorVentaSecundaria = responseCp.respuesta.intervinientes.cuitCorredorVentaSecundaria,
                            CuitRepresentanteEntregador = responseCp.respuesta.intervinientes.cuitRepresentanteEntregador,
                            CuitRepresentanteRecibidor = responseCp.respuesta.intervinientes.cuitRepresentanteRecibidor,
                            CuitRemitenteComercialVentaSecundaria2 = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaSecundaria2,

                            //datosCarga
                            Material = responseCp.respuesta.datosCarga.codGrano,
                            PesoBruto = responseCp.respuesta.datosCarga.pesoBruto,
                            PesoTara = responseCp.respuesta.datosCarga.pesoTara,
                            Cosecha = responseCp.respuesta.datosCarga.cosecha,

                            //destino
                            CuitDestino = responseCp.respuesta.destino.cuit,
                            LocalidadDestino = responseCp.respuesta.destino.codLocalidad,
                            ProvinciaDestino = responseCp.respuesta.destino.codProvincia,
                            PlantaDestino = responseCp.respuesta.destino.planta,

                            //destinatario
                            CuitDestinatario = responseCp.respuesta.destinatario.cuit,

                            //transporte
                            CuitTransportista = responseCp.respuesta.transporte.cuitTransportista,
                            Dominio = responseCp.respuesta.transporte.dominio != null && responseCp.respuesta.transporte.dominio.Length > 0 ? string.Join(",", responseCp.respuesta.transporte.dominio) : string.Empty,
                            FechaPartida = responseCp.respuesta.transporte.fechaHoraPartida,
                            KmRecorrer = responseCp.respuesta.transporte.kmRecorrer,
                            CodigoTurno = responseCp.respuesta.transporte.codigoTurno,

                            CuitChofer = responseCp.respuesta.transporte.cuitChofer,
                            Tarifa = Convert.ToDouble(responseCp.respuesta.transporte.tarifa),
                            CuitPagadorFlete = responseCp.respuesta.transporte.cuitPagadorFlete,
                            CuitIntermediarioFlete = responseCp.respuesta.transporte.cuitIntermediarioFlete,
                            MercaderiaFumigada = responseCp.respuesta.transporte.mercaderiaFumigada,
                            FechaUltimaActualizacion = comando.FechaUltimaActualizacion,
                            Pdf = responseCp?.respuesta?.pdf,
                            TarifaReferencia = Convert.ToDouble(responseCp.respuesta.transporte.tarifaReferencia),
                            FechaCacheado = DateTime.Now,

                        };
                        if (cartaPorte != null)
                        {
                            Conversor.Convertir(cartaPorteRequest, cartaPorte);
                            if (cartaPorteRequest.Pdf != null)
                            {
                                cartaPorte.Pdf = cartaPorteRequest.Pdf;
                            }
                        }
                        else
                        {
                            cartaPorte = Repositorio.Agregar(cartaPorteRequest);
                        }
                        Repositorio.GuardarCambios();

                        if (comando.ConsultaMinima)
                        {
                            var material = Repositorio.Obtener<Material>(x => x.CodigoEspecie == cartaPorte.Material && x.Activo);
                            resultado.Cpe = new CartaPorteDto
                            {
                                NroOrden = cartaPorte.NroOrden,
                                Cupo = cartaPorte.CodigoTurno,
                                Material = material.Descripcion,
                                MaterialId = material.Id,
                                NroCartaPorte = cartaPorte.NroCTG.ToString(),
                                Patente = cartaPorte.Dominio.Split(',').FirstOrDefault(),
                                Sucursal = cartaPorte.Sucursal,
                                Cpe = true,
                                EstadoCpe = cartaPorteRequest.Estado,
                                Vehiculos = new List<VehiculoDto>() { new VehiculoDto { Patente = cartaPorte?.Dominio?.Split(',')?.FirstOrDefault(),
                                PatenteAcoplado = cartaPorte.Dominio.Split(',').Length > 1 ? cartaPorte.Dominio.Split(',')[1] : string.Empty,
                                PatenteAcoplado2 = cartaPorte.Dominio.Split(',').Length > 2 ? cartaPorte.Dominio.Split(',').LastOrDefault() : string.Empty
                                }}
                            };
                            return resultado;
                        }

                        if (comando.ConsultaImagenCpe)
                        {
                            if (responseCp?.respuesta?.pdf != null)
                            {
                                try
                                {
                                    using (var document = PdfDocument.Load(new MemoryStream(responseCp.respuesta.pdf)))
                                    {
                                        var dpix = ConfigurationManager.AppSettings["PdfCpeDpiX"];
                                        var dpiy = ConfigurationManager.AppSettings["PdfCpeDpiY"];

                                        var image = document.Render(0, string.IsNullOrEmpty(dpix) ? 600 : Convert.ToInt32(dpix), string.IsNullOrEmpty(dpiy) ? 600 : Convert.ToInt32(dpiy), PdfRenderFlags.ForPrinting | PdfRenderFlags.CorrectFromDpi);
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                            resultado.PdfImage = ms.ToArray();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, $"Error al Convertir PDF en JPG {comando.NroCtg}");
                                    resultado.Errores.Add("4", "Error al Convertir PDF en JPG");
                                }
                            }
                            else
                            {
                                resultado.Errores.Add("3", "No se pudo obtener la imagen de la CP desde SCATO");
                            }
                            return resultado;
                        }

                        var localidad = cartaPorte.Localidad.Value.ToString();
                        var localidadObj = Repositorio.Listar<Localidad>(x => x.CodigoAfip == localidad).FirstOrDefault();

                        Localidad localidadDto = null;
                        if (localidadObj == null)
                        {
                            localidadDto = ObtenerLocalidadAfip(auth, localidad, cartaPorte.Provincia.Value);
                        } 
                        else
                        {
                            localidadDto = localidadObj;
                        }

                        var cuitRepresentanteEntregador = cartaPorte.CuitRepresentanteEntregador.ToString();
                        var cuitRepresentanteRecibidor = cartaPorte.CuitRepresentanteRecibidor.ToString();
                        var cuitchofer = cartaPorte.CuitChofer.ToString();
                        var cuitOrigen = responseCp.respuesta.origen.cuit.ToString() ?? string.Empty;
                        var titularCartaPorte = ObtenerProveedor(cuitOrigen, resultado, Textos.CartaPorte_TitularCartaPorte, false, false, true);
                        var categoriaStr = cartaPorte.NroCTG.ToString().Substring(0, 3).EndsWith("01") ? "PRODUCTOR" : "OPERADOR";
                        var categoria = Repositorio.Obtener<Categoria>(x => x.Clasificacion == categoriaStr);
                        var entregador = Repositorio.Listar<Entregador>(x => x.Cuil.Replace("-", "") == cuitRepresentanteEntregador && x.Activo).LastOrDefault();
                        var patentes = cartaPorte?.Dominio?.Split(',');
                        var representanteRecibidor = Repositorio.Listar<Entregador>(x => x.Cuil.Replace("-", "") == cuitRepresentanteRecibidor && x.Activo).LastOrDefault();

                        var entidad = new Dominio.Entidades.CartaPorte
                        {
                            Id = cartaPorte.Id,
                            Cpe = true,
                            NroCartaPorte = cartaPorte.NroCTG.ToString(),
                            FechaCP = cartaPorte.FechaCP.Value,
                            FechaEmision = cartaPorte.FechaEmision.Value,
                            FechaVto = cartaPorte.FechaVto.Value,
                            Categoria = categoria,

                            //datos traslado
                            TitularCartaPorte = titularCartaPorte,
                            Entregador = entregador is null ? Repositorio.Listar<Entregador>(x => x.RazonSocial.ToUpper().Contains("SIN ENTREGA")).LastOrDefault() : entregador,
                            Intermediario = ObtenerProveedor(cartaPorte.CuitIntermediario.ToString(), resultado, Textos.CartaPorte_Intermediario, false, false, true),
                            RtteComercial = ObtenerProveedor(cartaPorte.CuitRemitenteComercialVentaPrimaria.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                            RtteComercialVentaSecundaria = ObtenerProveedor(cartaPorte.CuitRemitenteComercialVentaSecundaria.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                            RtteComercialProductor = ObtenerProveedor(cartaPorte.CuitRemitenteComercialProductor.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                            AgenteCompras = ObtenerProveedor(cartaPorte.CuitMercadoATermino.ToString(), resultado, Textos.CartaPorte_AgenteCompras, false, false, true),
                            CorredorVendedor = ObtenerProveedor(cartaPorte.CuitCorredorVentaPrimaria.ToString(), resultado, Textos.CartaPorte_CorredorVendedor, false, true, false),
                            CorredorVendedorSecundario = ObtenerProveedor(cartaPorte.CuitCorredorVentaSecundaria.ToString(), resultado, Textos.CartaPorte_CorredorVendedor, false, true, false),
                            Corredor = ObtenerProveedor(cartaPorte?.CuitCorredorVentaPrimaria?.ToString(), resultado, Textos.Corredor_primario, false, true, false),
                            RtteComercialVentaSecundaria2 = ObtenerProveedor(cartaPorte?.CuitRemitenteComercialVentaSecundaria2.ToString(), resultado, Textos.Rtte_comercial_venta_secundaria_2, false, false, true),

                            //origen
                            Procedencia = localidadDto,
                            Material = Repositorio.Obtener<Material>(x => x.CodigoEspecie == cartaPorte.Material && x.Activo),

                            Vehiculos = new List<Vehiculo>
                        {
                           new Vehiculo
                           {
                               Patente = patentes?.FirstOrDefault(),
                               PatenteAcoplado = patentes.Length > 1 ? patentes[1] : string.Empty,
                               PatenteAcoplado2 = patentes.Length > 2 ? patentes.LastOrDefault() : string.Empty,
                               PesoBrutoOrigen = cartaPorte.PesoBruto ?? 0,
                               PesoTaraOrigen = cartaPorte.PesoTara ?? 0,
                               PesoNetoOrigen = (cartaPorte.PesoBruto ?? 0) - (cartaPorte.PesoTara ?? 0)
                           }
                        },
                            Destinatario = ObtenerProveedor(cartaPorte.CuitDestinatario.ToString(), resultado, Textos.CartaPorte_Destinatario, false, false, true),
                            Transportista = ObtenerTransportista(cartaPorte.CuitTransportista.ToString(), resultado),
                            KmRecorrer = cartaPorte.KmRecorrer,
                            TarifaTonelada = (decimal)cartaPorte.Tarifa,
                            TarifaReferencia = (decimal)cartaPorte.TarifaReferencia,
                            Chofer = Repositorio.Obtener<Chofer>(x => x.Cuil.Replace("-", "") == cuitchofer),

                            IntermediarioFlete = ObtenerProveedor(cartaPorte.CuitIntermediarioFlete.ToString(), resultado, Textos.CartaPorte_Intermediario, false, false, true),
                            Sucursal = cartaPorte.Sucursal,
                            Cosecha = cartaPorteRequest.Cosecha.HasValue ? cartaPorteRequest.Cosecha.Value.ToString() : string.Empty,
                            Observacion = cartaPorte.Observacion,
                            PagadorFlete = ObtenerProveedor(cartaPorte.CuitPagadorFlete.ToString(), resultado, Textos.CartaPorte_Transportista_Pagador_Flete, false, false, true),
                            RepresentanteRecibidor = representanteRecibidor,
                        };

                        resultado.Cpe = Conversor.Convertir<Dominio.Entidades.CartaPorte, CartaPorteDto>(entidad);
                        resultado.Cpe.NroOrden = cartaPorteRequest.NroOrden;
                        resultado.Cpe.Procedencia = localidadDto?.Descripcion;
                        resultado.Cpe.ProcedenciaId = localidadDto?.Id ?? 0;
                        resultado.Cpe.DestinoId = centro.Id;
                        resultado.Cpe.Destino = centro.Descripcion;
                        resultado.Cpe.Cupo = cartaPorte.CodigoTurno;
                        resultado.Cpe.CodEstab = cartaPorte.PlantaOrigen.HasValue ? cartaPorte.PlantaOrigen.ToString() : "99999";
                        resultado.Cpe.TitularCartaPorteId = titularCartaPorte != null ? titularCartaPorte.Id : default(int);
                        resultado.Cpe.TitularCartaPorte = titularCartaPorte != null ? titularCartaPorte.Descripcion : string.Empty;
                        resultado.Cpe.TipoVehiculo = TipoVehiculo.Camión;
                        resultado.Cpe.EstadoCpe = cartaPorteRequest.Estado;
                    }
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo consultar la CTG {0}", comando.NroCtg);
                resultado.Errores.Add("2", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo consultar la CTG {0}", comando.NroCtg);
                resultado.Errores.Add("2", Textos.Error_Generico);
            }
            return resultado;
        }

        private Localidad ObtenerLocalidadAfip(Auth auth,string localidadAfip, int provinciaAfip)
        {
            var provinciaObj = Repositorio.Listar<Provincia>(x => x.CodigoAfip == provinciaAfip).FirstOrDefault();
            var responseLoc = serviceAfipCPDigital.consultarLocalidadesPorProvincia(new consultarLocalidadesPorProvinciaRequest()
            {
                auth = auth,
                solicitud = new ConsultarLocalidadesPorProvinciaSolicitud()
                {
                    codProvincia = provinciaAfip
                }
            });

            if(responseLoc?.respuesta?.errores?.Length == 0)
            {
                var loc = responseLoc.respuesta.localidad.FirstOrDefault(x => x.codigo == localidadAfip);

                if(loc != null)
                {
                    var localidad = new Localidad
                    {
                        Provincia = provinciaObj,
                        CodigoAfip = loc.codigo,
                        Descripcion = loc.descripcion.Length > 50 ? loc.descripcion.Substring(0, 50) : loc.descripcion
                    };
                    Repositorio.Agregar(localidad);
                    Repositorio.GuardarCambios();
                    return localidad;
                }
            }

            return null;
        }

        private Proveedor ObtenerProveedor(string cuitSinGuiones, Resultado resultado, string nombreDato, bool am, bool cm, bool pr)
        {
            if (!string.IsNullOrEmpty(cuitSinGuiones) && cuitSinGuiones.Trim() != "0")
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
        private Transportista ObtenerTransportista(string cuitTransportista, Resultado resultado)
        {
            if (!string.IsNullOrEmpty(cuitTransportista))
            {
                var cuil = cuitTransportista.Substring(0, 2) + "-" + cuitTransportista.Substring(2, 8) + "-" + cuitTransportista.Substring(10, 1);
                var transportista = Repositorio.Listar<Transportista>(x => x.Cuit == cuil).LastOrDefault();
                if (transportista == null)
                {
                    AgregarWarning(resultado, string.Format(Textos.Transportista_NoEncontrado, cuil));
                }
                return transportista;
            }
            return null;
        }

        private CartaPorteDto ConvertirCartaPorteDto(Auth auth, CartaPorteElectronica cartaPorte, Centro centro, Resultado resultado,bool consultaMinima)
        {
            if (consultaMinima)
            {
                var material = Repositorio.Obtener<Material>(x => x.CodigoEspecie == cartaPorte.Material && x.Activo);
                return new CartaPorteDto {
                    NroOrden = cartaPorte.NroOrden,
                    Cupo = cartaPorte.CodigoTurno,
                    Material = material.Descripcion,
                    MaterialId = material.Id,
                    NroCartaPorte = cartaPorte.NroCTG.ToString(),
                    Patente = cartaPorte.Dominio.Split(',').FirstOrDefault(),
                    Sucursal = cartaPorte.Sucursal,
                    Cpe = true,
                    EstadoCpe = cartaPorte.Estado,
                    Vehiculos = new List<VehiculoDto>() { 
                        new VehiculoDto { 
                            Patente = cartaPorte?.Dominio?.Split(',')?.FirstOrDefault(),
                            PatenteAcoplado = cartaPorte?.Dominio?.Split(',')?.Length > 1 ? cartaPorte?.Dominio?.Split(',')?.LastOrDefault() : string.Empty
                        } 
                    }
                };
                
            }
            var localidad = cartaPorte.Localidad.Value.ToString();
            var provincia = cartaPorte.Provincia.Value.ToString();

            var localidadObj = Repositorio.Listar<Localidad>(x => x.CodigoAfip == localidad).FirstOrDefault();
            Localidad localidadDto = null;
            if (localidadObj == null)
            {
                localidadDto = ObtenerLocalidadAfip(auth, localidad, cartaPorte.Provincia.Value);
            }
            else
            {
                localidadDto = localidadObj;
            }

            var cuitRepresentanteEntregador = cartaPorte.CuitRepresentanteEntregador.ToString();
            var cuitRepresentanteRecibidor = cartaPorte.CuitRepresentanteRecibidor.ToString();
            var cuitchofer = cartaPorte.CuitChofer.ToString();
            var cuitOrigen = cartaPorte.CuitOrigen.ToString() ?? string.Empty;
            var titularCartaPorte = Repositorio.Obtener<Proveedor>(x => x.Cuil.Replace("-", "") == cuitOrigen && x.PR);
            var categoriaStr = cartaPorte.NroCTG.ToString().Substring(0, 3).EndsWith("01") ? "PRODUCTOR" : "OPERADOR";
            var categoria = Repositorio.Obtener<Categoria>(x => x.Clasificacion == categoriaStr);
            var entregador = Repositorio.Listar<Entregador>(x => x.Cuil.Replace("-", "") == cuitRepresentanteEntregador && x.Activo).LastOrDefault();
            var representanteRecibidor = Repositorio.Listar<Entregador>(x => x.Cuil.Replace("-", "") == cuitRepresentanteRecibidor && x.Activo).LastOrDefault();

            var entidad = new Dominio.Entidades.CartaPorte
            {
                Id = cartaPorte.Id,
                Cpe = true,
                NroCartaPorte = cartaPorte.NroCTG.ToString(),
                FechaCP = cartaPorte.FechaCP.Value,
                FechaEmision = cartaPorte.FechaEmision.Value,
                FechaVto = cartaPorte.FechaVto.Value,
                Categoria = categoria,

                //datos traslado
                TitularCartaPorte = ObtenerProveedor(cuitOrigen, resultado, Textos.CartaPorte_TitularCartaPorte, false, false, true),
                Entregador = entregador is null ? Repositorio.Listar<Entregador>(x => x.RazonSocial.ToUpper().Contains("SIN ENTREGA")).LastOrDefault() : entregador,
                Intermediario = ObtenerProveedor(cartaPorte.CuitIntermediario.ToString(), resultado, Textos.CartaPorte_Intermediario, false, false, true),
                RtteComercial = ObtenerProveedor(cartaPorte.CuitRemitenteComercialVentaPrimaria.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                RtteComercialVentaSecundaria = ObtenerProveedor(cartaPorte.CuitRemitenteComercialVentaSecundaria.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                RtteComercialProductor = ObtenerProveedor(cartaPorte.CuitRemitenteComercialProductor.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                AgenteCompras = ObtenerProveedor(cartaPorte.CuitMercadoATermino.ToString(), resultado, Textos.CartaPorte_AgenteCompras, false, false, true),
                CorredorVendedor = ObtenerProveedor(cartaPorte.CuitCorredorVentaPrimaria.ToString(), resultado, Textos.CartaPorte_CorredorVendedor, false, true, false),
                CorredorVendedorSecundario = ObtenerProveedor(cartaPorte.CuitCorredorVentaSecundaria.ToString(), resultado, Textos.CartaPorte_CorredorVendedor, false, true, false),
                Corredor = ObtenerProveedor(cartaPorte?.CuitCorredorVentaPrimaria?.ToString(), resultado, Textos.Corredor_primario, false, true, false),
                RtteComercialVentaSecundaria2 = ObtenerProveedor(cartaPorte?.CuitRemitenteComercialVentaSecundaria2.ToString(), resultado, Textos.Rtte_comercial_venta_secundaria_2, false, false, true),

                //origen
                Procedencia = localidadDto,
                Material = Repositorio.Obtener<Material>(x => x.CodigoEspecie == cartaPorte.Material && x.Activo),

                Vehiculos = new List<Vehiculo>
                        {
                           new Vehiculo
                           {
                               Patente = cartaPorte?.Dominio?.Split(',')?.FirstOrDefault(),
                               PatenteAcoplado = cartaPorte?.Dominio?.Split(',')?.Length > 1 ? cartaPorte?.Dominio?.Split(',')?.LastOrDefault() : string.Empty,
                               PesoBrutoOrigen = cartaPorte.PesoBruto ?? 0,
                               PesoTaraOrigen = cartaPorte.PesoTara ?? 0,
                               PesoNetoOrigen = (cartaPorte.PesoBruto ?? 0) - (cartaPorte.PesoTara ?? 0)
                           }
                        },
                Destinatario = ObtenerProveedor(cartaPorte.CuitDestinatario.ToString(), resultado, Textos.CartaPorte_Destinatario, false, false, true),
                Transportista = ObtenerTransportista(cartaPorte.CuitTransportista.ToString(), resultado),
                KmRecorrer = cartaPorte.KmRecorrer,
                TarifaTonelada = (decimal)cartaPorte.Tarifa,
                TarifaReferencia = (decimal)cartaPorte.TarifaReferencia,
                Chofer = Repositorio.Obtener<Chofer>(x => x.Cuil.Replace("-", "") == cuitchofer),

                IntermediarioFlete = ObtenerProveedor(cartaPorte.CuitIntermediarioFlete.ToString(), resultado, Textos.CartaPorte_Intermediario, false, false, true),
                Sucursal = cartaPorte.Sucursal,
                Cosecha = cartaPorte.Cosecha.HasValue ? cartaPorte.Cosecha.Value.ToString() : string.Empty,
                Observacion = cartaPorte.Observacion,
                PagadorFlete = ObtenerProveedor(cartaPorte.CuitPagadorFlete.ToString(), resultado, Textos.CartaPorte_Transportista_Pagador_Flete, false, false, true),
                RepresentanteRecibidor = representanteRecibidor,

            };

            var cpe = Conversor.Convertir<Dominio.Entidades.CartaPorte, CartaPorteDto>(entidad);
            cpe.NroOrden = cartaPorte.NroOrden;
            cpe.Procedencia = localidadDto?.Descripcion;
            cpe.ProcedenciaId = localidadDto?.Id ?? 0;
            cpe.DestinoId = centro.Id;
            cpe.Destino = centro.Descripcion;
            cpe.Cupo = cartaPorte.CodigoTurno;
            cpe.CodEstab = cartaPorte.PlantaOrigen.HasValue ? cartaPorte.PlantaOrigen.ToString() : "99999";
            cpe.TitularCartaPorteId = titularCartaPorte != null ? titularCartaPorte.Id : default(int);
            cpe.TitularCartaPorte = titularCartaPorte != null ? titularCartaPorte.Descripcion : string.Empty;
            cpe.EstadoCpe = cartaPorte.Estado;

            return cpe;
        }

        public ResultadoCartaPorteElectronica ObtenerCTGsPorNroDeOperativo(long nroOperativo, Auth auth)
        {
            var resultado = new ResultadoCartaPorteElectronica();
            var erroresNobloqueantes = new List<string>() { "550" }; //no se pudo generar el pdf

            try
            {
                // Consulta por nro de operativo
                var requestPorOperativo = new consultaCPEFerroviariaPorNroOperativoRequest
                {
                    auth = auth,
                    solicitud = new ConsultaCPEFerroviariaPorNroOperativoSolicitud
                    {
                        nroOperativo = nroOperativo
                    }
                };

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var responseCpPorOperativo = serviceAfipCPDigital.consultaCPEFerroviariaPorNroOperativo(requestPorOperativo);
                if (responseCpPorOperativo.respuesta == null)
                {
                    resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                    Log.Debug($"Consulta de CPE por Nro de operativo ctg {nroOperativo} sin respuesta");
                    return resultado;

                }
                if (responseCpPorOperativo.respuesta != null && responseCpPorOperativo.respuesta.errores != null && responseCpPorOperativo.respuesta.errores.Any())
                {
                    if (!erroresNobloqueantes.Any(x => responseCpPorOperativo.respuesta.errores.Any(y => y.codigo == x)))
                    {
                        foreach (var error in responseCpPorOperativo.respuesta.errores)
                        {
                            resultado.Errores.Add("2", error.descripcion);
                        }
                    }

                    foreach (var error in responseCpPorOperativo.respuesta.errores)
                    {
                        Log.Error(string.Format("ProcesadorConsultarCPDigital - ({0}) {1}", error.codigo, error.descripcion));
                    }
                }
                resultado.CTGsDeOperativo = responseCpPorOperativo?.respuesta?.cartaPorte?.Select(cp => cp.nroCTG).ToList();
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo consultar el numero de operativo {0}", nroOperativo);
                resultado.Errores.Add("2", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo consultar el numero de operativo {0}", nroOperativo);
                resultado.Errores.Add("2", Textos.Error_Generico);
            }

            return resultado;
        }

        public ResultadoCartaPorteElectronica GuardarCartaPorteElectronica(long nroCTG, Auth auth, Centro centro)
        {
            var resultado = new ResultadoCartaPorteElectronica();
            var cartaPorte = Repositorio.Obtener<CartaPorteElectronica>(x => x.NroCTG == nroCTG);
            var continuar = true;
            var erroresNobloqueantes = new List<string>() { "550" }; //no se pudo generar el pdf

            try
            {
                var request = new consultarCPEFerroviariaRequest
                {
                    auth = auth,
                    solicitud = new ConsultarFerroviariaSolicitud
                    {
                        nroCTG = nroCTG,
                        nroCTGSpecified = true
                    }
                };

                Log.Debug(request.ToXml());

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var responseCp = serviceAfipCPDigital.consultarCPEFerroviaria(request);
                if (responseCp.respuesta == null)
                {
                    resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                    Log.Debug($"Consulta de CPE ctg {nroCTG} sin respuesta");
                    return resultado;
                }

                if (responseCp.respuesta != null && responseCp.respuesta.errores != null && responseCp.respuesta.errores.Any())
                {
                    if (!erroresNobloqueantes.Any(x => responseCp.respuesta.errores.Any(y => y.codigo == x)))
                    {
                        foreach (var error in responseCp.respuesta.errores)
                        {
                            resultado.Errores.Add("2", error.descripcion);
                        }

                        continuar = false;
                    }

                    foreach (var error in responseCp.respuesta.errores)
                    {
                        Log.Error(string.Format("ProcesadorConsultarCPDigital - ({0}) {1}", error.codigo, error.descripcion));
                    }
                }

                if (responseCp.respuesta != null && continuar)
                {
                    var cartaPorteRequest = new CartaPorteElectronica
                    {
                        //cabecera
                        TipoCartaPorte = responseCp.respuesta.cabecera.tipoCartaPorte,
                        Sucursal = responseCp.respuesta.cabecera.sucursal,
                        NroOrden = responseCp.respuesta.cabecera.nroOrden,
                        //Planta = responseCp.respuesta.cabecera.planta,
                        NroCTG = responseCp.respuesta.cabecera.nroCTG,
                        FechaEmision = responseCp.respuesta.cabecera.fechaEmision,
                        Estado = responseCp.respuesta.cabecera.estado,
                        FechaCP = responseCp.respuesta.cabecera.fechaInicioEstado,
                        FechaVto = responseCp.respuesta.cabecera.fechaVencimiento,

                        //origen
                        Provincia = responseCp.respuesta.origen.codProvincia,
                        Localidad = responseCp.respuesta.origen.codLocalidad,
                        Domicilio = responseCp.respuesta.origen.domicilio,
                        PlantaOrigen = responseCp.respuesta.origen.planta,

                        //correspondeRetiroProductor
                        RetiroProductor = responseCp.respuesta.correspondeRetiroProductor,

                        //retiroProductor

                        //CertificadoCOE = responseCp.respuesta.retiroProductor!= null ? responseCp.respuesta.retiroProductor.certificadoCOE: (long?)null,
                        CuitRemitenteComercialProductor = responseCp.respuesta.retiroProductor != null ? responseCp.respuesta.retiroProductor.cuitRemitenteComercialProductor : (long?)null,

                        //intervinientes
                        CuitRepresentanteRecibidor = responseCp.respuesta.intervinientes.cuitRepresentanteRecibidor,
                        CuitIntermediario = responseCp?.respuesta?.retiroProductor?.cuitRemitenteComercialProductor ?? 0,
                        CuitRemitenteComercialVentaPrimaria = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaPrimaria,
                        CuitRemitenteComercialVentaSecundaria = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaSecundaria,
                        CuitMercadoATermino = responseCp.respuesta.intervinientes.cuitMercadoATermino,
                        CuitCorredorVentaPrimaria = responseCp.respuesta.intervinientes.cuitCorredorVentaPrimaria,
                        CuitCorredorVentaSecundaria = responseCp.respuesta.intervinientes.cuitCorredorVentaSecundaria,
                        CuitRepresentanteEntregador = responseCp.respuesta.intervinientes.cuitRepresentanteEntregador,
                        CuitRemitenteComercialVentaSecundaria2 = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaSecundaria2,

                        //datosCarga
                        Material = responseCp.respuesta.datosCarga.codGrano,
                        PesoBruto = responseCp.respuesta.datosCarga.pesoBruto,
                        PesoTara = responseCp.respuesta.datosCarga.pesoTara,

                        //destino
                        CuitDestino = responseCp.respuesta.destino.cuit,
                        LocalidadDestino = responseCp.respuesta.destino.codLocalidad,
                        ProvinciaDestino = responseCp.respuesta.destino.codProvincia,
                        PlantaDestino = responseCp.respuesta.destino.planta,

                        //destinatario
                        CuitDestinatario = responseCp.respuesta.destinatario.cuit,

                        //transporte
                        CuitTransportista = responseCp.respuesta.transporte.cuitTransportista,
                        Dominio = responseCp.respuesta.transporte.nroVagon != 0 ? string.Join(",", responseCp.respuesta.transporte.nroVagon) : string.Empty,
                        FechaPartida = responseCp.respuesta.transporte.fechaHoraPartidaTren,
                        KmRecorrer = responseCp.respuesta.transporte.kmRecorrer,
                        CuitPagadorFlete = responseCp.respuesta.transporte.cuitPagadorFlete,
                        MercaderiaFumigada = responseCp.respuesta.transporte.mercaderiaFumigada,
                        CuitTransportistaTramo2 = responseCp.respuesta.transporte.cuitTransportistaTramo2,

                        //cosecha
                        Cosecha = responseCp.respuesta.datosCarga.cosecha,

                        //NroOperativo
                        NroOperativo = responseCp.respuesta.transporte.nroOperativo,

                        //Ramal
                        RamalFerroviario = responseCp?.respuesta?.transporte?.ramal?.codigo,

                        //NumeroOperativo
                        NumeroPrecinto = responseCp?.respuesta?.transporte?.nroPrecinto,

                        //Pdf
                        Pdf = responseCp?.respuesta?.pdf,

                        //Onbservacion
                        Observacion = responseCp?.respuesta?.cabecera?.observaciones,

                        FechaCacheado = DateTime.Now,

                    };
                    if (cartaPorte != null)
                    {
                        Conversor.Convertir(cartaPorteRequest, cartaPorte);
                        if(cartaPorteRequest.Pdf != null)
                        {
                            cartaPorte.Pdf = cartaPorteRequest.Pdf;
                        }
                    }
                    else
                    {
                        cartaPorte = Repositorio.Agregar(cartaPorteRequest);
                    }
                    Repositorio.GuardarCambios();

                    var localidad = cartaPorte.Localidad.Value.ToString();
                    var provincia = cartaPorte.Provincia.Value.ToString();

                    var localidadObj = Repositorio.Listar<Localidad>(x => x.CodigoAfip == localidad).FirstOrDefault();
                    Localidad localidadDto = null;
                    if (localidadObj == null)
                    {
                        localidadDto = new Localidad
                        {
                            Id = localidadObj.Id,
                            Provincia = new Provincia { Id = localidadObj.Provincia.Id, CodigoAfip = localidadObj.Provincia.CodigoAfip, Descripcion = localidadObj.Provincia.Descripcion },
                            CodigoAfip = localidadObj.CodigoAfip,
                            Descripcion = localidadObj.CodigoAfip + " - " + localidadObj.Descripcion + "(" + localidadObj.Provincia.Descripcion + ")"
                        };
                    } 
                    else
                    {
                        localidadDto = localidadObj;
                    }

                    var cuitRepresentanteEntregador = cartaPorte.CuitRepresentanteEntregador.ToString();
                    var cuitRepresentanteRecibidor = cartaPorte.CuitRepresentanteRecibidor.ToString();
                    var cuitchofer = cartaPorte.CuitChofer.ToString();
                    var cuitOrigen = responseCp.respuesta.origen.cuit.ToString() ?? string.Empty;
                    var titularCartaPorte = Repositorio.Obtener<Proveedor>(x => x.Cuil.Replace("-", "") == cuitOrigen && x.PR);
                    var categoriaStr = cartaPorte.NroCTG.ToString().Substring(0, 3).EndsWith("01") ? "PRODUCTOR" : "OPERADOR";
                    var ramalFerroviarioAfip = Convert.ToInt32(cartaPorteRequest.RamalFerroviario);
                    var categoria = Repositorio.Obtener<Categoria>(x => x.Clasificacion == categoriaStr);
                    var entregador = Repositorio.Listar<Entregador>(x => x.Cuil.Replace("-", "") == cuitRepresentanteEntregador && x.Activo).LastOrDefault();
                    var ramalFerroviario = Repositorio.Obtener<RamalFerroviario>(x => x.CodigoAfip == ramalFerroviarioAfip);
                    var representanteRecibidor = Repositorio.Listar<Entregador>(x => x.Cuil.Replace("-", "") == cuitRepresentanteRecibidor && x.Activo).LastOrDefault();

                    var entidad = new Dominio.Entidades.CartaPorte
                    {
                        Id = cartaPorte.Id,
                        Cpe = true,
                        NroCartaPorte = cartaPorte.NroCTG.ToString(),
                        FechaCP = cartaPorte.FechaCP.Value,
                        FechaEmision = cartaPorte.FechaEmision.Value,
                        FechaVto = cartaPorte.FechaVto.Value,
                        Categoria = categoria,

                        //datos traslado
                        TitularCartaPorte = ObtenerProveedor(cuitOrigen, resultado, Textos.CartaPorte_TitularCartaPorte, false, false, true),
                        Entregador = entregador is null ? Repositorio.Listar<Entregador>(x => x.RazonSocial.ToUpper().Contains("SIN ENTREGA")).LastOrDefault() : entregador,
                        Intermediario = ObtenerProveedor(cartaPorte.CuitIntermediario.ToString(), resultado, Textos.CartaPorte_Intermediario, false, false, true),
                        RtteComercial = ObtenerProveedor(cartaPorte.CuitRemitenteComercialVentaPrimaria.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                        RtteComercialVentaSecundaria = ObtenerProveedor(cartaPorte.CuitRemitenteComercialVentaSecundaria.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                        RtteComercialProductor = ObtenerProveedor(cartaPorte.CuitRemitenteComercialProductor.ToString(), resultado, Textos.CartaPorte_RtteComercial, false, false, true),
                        AgenteCompras = ObtenerProveedor(cartaPorte.CuitMercadoATermino.ToString(), resultado, Textos.CartaPorte_AgenteCompras, false, false, true),
                        CorredorVendedor = ObtenerProveedor(cartaPorte.CuitCorredorVentaPrimaria.ToString(), resultado, Textos.CartaPorte_CorredorVendedor, false, true, false),
                        CorredorVendedorSecundario = ObtenerProveedor(cartaPorte.CuitCorredorVentaSecundaria.ToString(), resultado, Textos.CartaPorte_CorredorVendedor, false, true, false),
                        Corredor = ObtenerProveedor(cartaPorte?.CuitCorredorVentaPrimaria?.ToString(), resultado, Textos.Corredor_primario, false, true, false),
                        RtteComercialVentaSecundaria2 = ObtenerProveedor(cartaPorte?.CuitRemitenteComercialVentaSecundaria2.ToString(), resultado, Textos.Rtte_comercial_venta_secundaria_2, false, false, true),

                        //origen
                        Procedencia = localidadDto,
                        Material = Repositorio.Obtener<Material>(x => x.CodigoEspecie == cartaPorte.Material && x.Activo),

                        Vehiculos = new List<Vehiculo>
                        {
                           new Vehiculo
                           {
                               Patente = cartaPorte?.Dominio?.Split(',')?.FirstOrDefault() != null && cartaPorte?.Dominio?.Split(',')?.FirstOrDefault().Length >= 7 ? cartaPorte?.Dominio?.Split(',')?.FirstOrDefault().Substring(0, 7) : cartaPorte?.Dominio?.Split(',')?.FirstOrDefault(),
                               PatenteAcoplado = cartaPorte.Dominio.Split(',').Length > 1 ? cartaPorte.Dominio.Split(',').LastOrDefault() : string.Empty,
                               PesoBrutoOrigen = cartaPorte.PesoBruto ?? 0,
                               PesoTaraOrigen = cartaPorte.PesoTara ?? 0,
                               PesoNetoOrigen = (cartaPorte.PesoBruto ?? 0) - (cartaPorte.PesoTara ?? 0)
                           }
                        },
                        Destinatario = ObtenerProveedor(cartaPorte.CuitDestinatario.ToString(), resultado, Textos.CartaPorte_Destinatario, false, false, true),
                        Transportista = ObtenerTransportista(cartaPorte.CuitTransportista.ToString(), resultado),
                        KmRecorrer = cartaPorte.KmRecorrer,
                        TarifaTonelada = (decimal)cartaPorte.Tarifa,
                        Chofer = Repositorio.Obtener<Chofer>(x => x.Cuil.Replace("-", "") == cuitchofer),

                        Sucursal = cartaPorte.Sucursal,
                        Cosecha = cartaPorteRequest.Cosecha.HasValue ? cartaPorteRequest.Cosecha.Value.ToString() : string.Empty,
                        TransportistaTramo2 = Convert.ToUInt64(cartaPorte.CuitTransportistaTramo2) == 0 ? null : ObtenerTransportista(cartaPorte.CuitTransportistaTramo2.ToString(), resultado),
                        PagadorFlete = ObtenerProveedor(cartaPorte.CuitPagadorFlete.ToString(), resultado, Textos.CartaPorte_Transportista_Pagador_Flete, false, false, true),
                        RepresentanteRecibidor = representanteRecibidor,

                    };

                    resultado.Cpe = Conversor.Convertir<Dominio.Entidades.CartaPorte, CartaPorteDto>(entidad);
                    resultado.Cpe.NroOrden = cartaPorteRequest.NroOrden;
                    resultado.Cpe.Procedencia = localidadDto?.Descripcion;
                    resultado.Cpe.ProcedenciaId = int.TryParse(localidadDto?.CodigoAfip, out int number) ? int.Parse(localidadDto?.CodigoAfip) : default(int);
                    resultado.Cpe.DestinoId = centro.Id;
                    resultado.Cpe.Destino = centro.Descripcion;
                    resultado.Cpe.Cupo = cartaPorte.CodigoTurno;
                    resultado.Cpe.CodEstab = cartaPorte.PlantaOrigen.HasValue ? cartaPorte.PlantaOrigen.ToString() : "99999";
                    resultado.Cpe.TitularCartaPorteId = titularCartaPorte != null ? titularCartaPorte.Id : default(int);
                    resultado.Cpe.TitularCartaPorte = titularCartaPorte != null ? titularCartaPorte.Descripcion : string.Empty;
                    resultado.Cpe.TipoVehiculo = TipoVehiculo.Tren;
                    resultado.Cpe.EstadoCpe = cartaPorteRequest.Estado;
                    resultado.Cpe.NumeroPrecinto = cartaPorteRequest.NumeroPrecinto;
                    resultado.Cpe.NumeroOperativo = cartaPorteRequest.NroOperativo;

                    if (!(ramalFerroviario is null))
                    {
                        resultado.Cpe.CodigoRamal = ramalFerroviario.Descripcion;
                        resultado.Cpe.CodigoRamalAfip = ramalFerroviario.CodigoAfip;
                        resultado.Cpe.CodigoRamalId = ramalFerroviario.Id;
                    }                    
                }

            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo consultar la CTG {0}", nroCTG);
                resultado.Errores.Add("2", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo consultar la CTG {0}", nroCTG);
                resultado.Errores.Add("2", Textos.Error_Generico);
            }
            
            return resultado;
        }
    }
}
