using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using PdfiumViewer;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCpOtrosPuertos : ProcesadorComando<CrearCpOtrosPuertos>
    {
        private readonly IServicioRepositorio svcRepo;
        private readonly IServicioComandos servicioComandos;
        public ProcesadorCrearCpOtrosPuertos(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioRepositorio svcRepo, IServicioComandos servicioComandos)
            : base(repositorio,conversor,log)
        {
            this.svcRepo = svcRepo;
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(CrearCpOtrosPuertos comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                if (comando.Dto != null)
                {
                    foreach (var dto in comando.Dto)
                    {

                        Log.Info("Se procedera a ejecutar ProcesadorCrearCpOtrosPuertos ");
                       
                        //var fotoCpOtrosPuertos = Repositorio.Obtener<DocumentoExterno>(x => x.NumeroDeDocumento == dto.NumeroCartaPorte);
                        var material = Repositorio.Obtener<Material>(x => x.CodigoONCCA == dto.ProductoCodigo);
                        var cartaPorteOtroPuertos = Repositorio.Obtener<CartaPorteOtrosPuertos>(x => x.NroCartaPorte == dto.NumeroCartaPorte);
                        
                        if (cartaPorteOtroPuertos == null)
                        {
                          
                            cartaPorteOtroPuertos = new CartaPorteOtrosPuertos
                            {
                                FechaCreacion = DateTime.Now,
                                UsuarioCreacion = comando.Usuario,
                                NroCartaPorte = dto.NumeroCartaPorte,
                                FechaCarga = dto.FechaCarga,
                                FechaVencimiento = dto.FechaVencimiento,
                                FechaArribo = dto.FechaArribo,
                                FechaDescarga = dto.FechaDescarga,
                                CEE = dto.CEE,
                                CTG = dto.CTG,
                                Establecimiento = dto.Establecimiento,
                                Planta = dto.Planta,
                                TitularCuit = dto.TitularCuit,
                                TitularRazonSocial = dto.TitularRazonSocial,
                                IntermediarioCuit = dto.IntermediarioCuit,
                                IntermediarioRazonSocial = dto.IntermediarioRazonSocial,
                                RemitenteCuit = dto.RemitenteCuit,
                                RemitenteRazonSocial = dto.RemitenteRazonSocial,
                                ProductoCodigo = dto.ProductoCodigo,
                                ProductoDescripcion = dto.ProductoDescripcion,
                                ProductoCosecha = ValidarFormatoCosecha(dto.ProductoCosecha),
                                Material_Id = material != null ? material.Id : (int?)null,
                                ProcedenciaCodigo = dto.ProcedenciaCodigo,
                                ProcedenciaLocalidad = dto.ProcedenciaLocalidad,
                                ProcedenciaPB = dto.ProcedenciaPB,
                                ProcedenciaPN = dto.ProcedenciaPN,
                                ProcedenciaPT = dto.ProcedenciaPT,
                                ProcedenciaProvincia = dto.ProcedenciaProvincia,
                                ProcedenciaCP = dto.ProcedenciaCP,
                                CorredorCuit = dto.CorredorCuit,
                                CorredorRazonSocial = dto.CorredorRazonSocial,
                                EntregadorCuit = dto.EntregadorCuit,
                                EntregadorRazonSocial = dto.EntregadorRazonSocial,
                                DestinatarioCuit = dto.DestinatarioCuit,
                                DestinatarioRazonSocial = dto.DestinatarioRazonSocial,
                                DestinoCuit = dto.DestinoCuit,
                                DestinoRazonSocial = dto.DestinoRazonSocial,
                                DestinoPB = dto.DestinoPB,
                                DestinoPN = dto.DestinoPN,
                                DestinoPT = dto.DestinoPT,
                                TransporteCuit = dto.TransporteCuit,
                                TransporteRazonSocial = dto.TransporteRazonSocial,
                                TransportePatente = dto.TrasportePatente,
                                ChoferCuit = dto.ChoferCuit,
                                ChoferRazonSocial = dto.ChoferRazonSocial,
                                MermaTotal = dto.MermaTotal,
                                NetoConvenido = dto.NetoConvenido,
                                DescuentosKgsHumedad = dto.DescuentosKgsHumedad,
                                DescuentosKgsCalidad = dto.DescuentosKgsCalidad,
                                DescuentosKgsSecada = dto.DescuentosKgsSecada,
                                TipoCartaPorte = dto.TipoCartaPorte,
                                Sucursal = dto.Sucursal,
                                NroOrden = dto.NroOrden,
                                Estado = dto.Estado,
                                Domicilio = dto.Domicilio,
                                PlantaOrigen = dto.PlantaOrigen,
                                RetiroProductor = dto.RetiroProductor,
                                CertificadoCOE = dto.CertificadoCOE,
                                CuitRemitenteComercialVentaPrimaria = dto.CuitRemitenteComercialVentaPrimaria,
                                RazonSocialRemitenteComercialVentaPrimaria = dto.RazonSocialRemitenteComercialVentaPrimaria,
                                CuitRemitenteComercialVentaSecundaria= dto.CuitRemitenteComercialVentaSecundaria,
                                RazonSocialRemitenteComercialVentaSecundaria = dto.RazonSocialRemitenteComercialVentaSecundaria,
                                CuitMercadoATermino = dto.CuitMercadoATermino,
                                RazonSocialMercadoATermino = dto.RazonSocialMercadoATermino,
                                CuitCorredorVentaSecundaria = dto.CuitCorredorVentaSecundaria,
                                RazonSocialCorredorVentaSecundaria = dto.RazonSocialCorredorVentaSecundaria,
                                PlantaDestino = dto.PlantaDestino,
                                KmRecorrer = dto.KmRecorrer,
                                Cupo = dto.Cupo,
                                Tarifa = dto.Tarifa,
                                CuitPagadorFlete = dto.CuitPagadorFlete,
                                RazonSocialPagadorFlete = dto.RazonSocialPagadorFlete,
                                MercaderiaFumigada = dto.MercaderiaFumigada,
                                CuitRepresentanteRecibidor = dto.CuitRepresentanteRecibidor,
                                RazonSocialRepresentanteRecibidor = dto.RazonSocialRepresentanteRecibidor,
                                CuitOrigen = dto.CuitOrigen,
                                Observacion = dto.Observacion,
                                FechaUltimaActualizacion = dto.FechaUltimaActualizacion,
                                NroOperativo = dto.NroOperativo,
                                CuitRemitenteComercialVentaSecundaria2 = dto.CuitRemitenteComercialVentaSecundaria2,
                                RazonSocialRemitenteComercialVentaSecundaria2 = dto.RazonSocialRemitenteComercialVentaSecundaria2,
                                RamalFerroviario = dto.RamalFerroviario,
                                NumeroPrecinto = dto.NumeroPrecinto,
                                Pdf = dto.Pdf,
                                EsSustentable = dto.EsSustentable,
                                CodigoEstablecimientoSustentable = dto.CodigoEstablecimientoSustentable
                            };

                           
                            cartaPorteOtroPuertos.Caracteristicas = new List<CaracteristicasCartaPorteOtrosPuertos>();

                            foreach (var item in dto.Caracteristicas)
                            {
                                cartaPorteOtroPuertos.Caracteristicas.Add(
                                    new CaracteristicasCartaPorteOtrosPuertos
                                    {
                                        CodigoExterno = item.CodigoExterno,
                                        Valor = item.Valor

                                    }
                                );
                            }

                            Repositorio.Agregar(cartaPorteOtroPuertos);

                            if (dto.EsSustentable)
                            {
                                //registrar en tabla sustentable
                                if (ValidarGrabarStock(dto))
                                {
                                    RegistroStockOtrosPuertos registroStockOtrosPuertos = new RegistroStockOtrosPuertos
                                    {
                                        CodigoEstablecimiento = cartaPorteOtroPuertos.CodigoEstablecimientoSustentable,
                                        Cosecha = cartaPorteOtroPuertos.ProductoCosecha,
                                        PesoNeto = decimal.Parse(cartaPorteOtroPuertos.DestinoPN),
                                        CartaPorteOtrosPuertos = cartaPorteOtroPuertos
                                    };
                                    Repositorio.Agregar(registroStockOtrosPuertos);
                                }
                            }
                        }
                        else
                        {
                            cartaPorteOtroPuertos.FechaUltimaModificacion = DateTime.Now;
                            cartaPorteOtroPuertos.UsuarioUltimaModificacion = comando.Usuario;
                            //    cartaPorteOtroPuertos.NumeroCartaPorte = dto.NumeroCartaPorte;
                            //    cartaPorteOtroPuertos.FechaCarga = dto.FechaCarga;
                            //    cartaPorteOtroPuertos.FechaVencimiento = dto.FechaVencimiento;
                            //    cartaPorteOtroPuertos.FechaArribo = dto.FechaArribo;
                            //    cartaPorteOtroPuertos.FechaDescarga = dto.FechaDescarga;
                            //    cartaPorteOtroPuertos.CEE = dto.CEE;
                            //    cartaPorteOtroPuertos.CTG = dto.CTG;
                            //    cartaPorteOtroPuertos.Establecimiento = dto.Establecimiento;
                            //    cartaPorteOtroPuertos.Planta = dto.Planta;
                            //    cartaPorteOtroPuertos.TitularCuit = dto.TitularCuit;
                            //    cartaPorteOtroPuertos.TitularRazonSocial = dto.TitularRazonSocial;
                            //    cartaPorteOtroPuertos.IntermediarioCuit = dto.IntermediarioCuit;
                            //    cartaPorteOtroPuertos.IntermediarioRazonSocial = dto.IntermediarioRazonSocial;
                            //    cartaPorteOtroPuertos.RemitenteCuit = dto.RemitenteCuit;
                            //    cartaPorteOtroPuertos.RemitenteRazonSocial = dto.RemitenteRazonSocial;
                            //    cartaPorteOtroPuertos.ProductoCodigo = dto.ProductoCodigo;
                            //    cartaPorteOtroPuertos.ProductoDescripcion = dto.ProductoDescripcion;
                            //    cartaPorteOtroPuertos.ProductoCosecha = dto.ProductoCosecha;
                            //    cartaPorteOtroPuertos.MaterialId = material != null ? material.Id : (int?)null;
                            //    cartaPorteOtroPuertos.ProcedenciaCodigo = dto.ProcedenciaCodigo;
                            //    cartaPorteOtroPuertos.ProcedenciaLocalidad = dto.ProcedenciaLocalidad;
                            //    cartaPorteOtroPuertos.ProcedenciaPB = dto.ProcedenciaPB;
                            //    cartaPorteOtroPuertos.ProcedenciaPN = dto.ProcedenciaPN;
                            //    cartaPorteOtroPuertos.ProcedenciaPT = dto.ProcedenciaPT;
                            //    cartaPorteOtroPuertos.ProcedenciaProvincia = dto.ProcedenciaProvincia;
                            //    cartaPorteOtroPuertos.ProcedenciaCP = dto.ProcedenciaCP;
                            //    cartaPorteOtroPuertos.CorredorCuit = dto.CorredorCuit;
                            //    cartaPorteOtroPuertos.CorredorRazonSocial = dto.CorredorRazonSocial;
                            //    cartaPorteOtroPuertos.EntregadorCuit = dto.EntregadorCuit;
                            //    cartaPorteOtroPuertos.EntregadorRazonSocial = dto.EntregadorRazonSocial;
                            //    cartaPorteOtroPuertos.DestinatarioCuit = dto.DestinatarioCuit;
                            //    cartaPorteOtroPuertos.DestinatarioRazonSocial = dto.DestinatarioRazonSocial;
                            //    cartaPorteOtroPuertos.DestinoCuit = dto.DestinoCuit;
                            //    cartaPorteOtroPuertos.DestinoRazonSocial = dto.DestinoRazonSocial;
                            //    cartaPorteOtroPuertos.DestinoPB = dto.DestinoPB;
                            //    cartaPorteOtroPuertos.DestinoPN = dto.DestinoPN;
                            //    cartaPorteOtroPuertos.DestinoPT = dto.DestinoPT;
                            //    cartaPorteOtroPuertos.TransporteCuit = dto.TransporteCuit;
                            //    cartaPorteOtroPuertos.TransporteRazonSocial = dto.TransporteRazonSocial;
                            //    cartaPorteOtroPuertos.TrasportePatente = dto.TrasportePatente;
                            //    cartaPorteOtroPuertos.ChoferCuit = dto.ChoferCuit;
                            //    cartaPorteOtroPuertos.ChoferRazonSocial = dto.ChoferRazonSocial;
                            //    cartaPorteOtroPuertos.MermaTotal = dto.MermaTotal;
                            //    cartaPorteOtroPuertos.NetoConvenido = dto.NetoConvenido;
                            //    cartaPorteOtroPuertos.DescuentosKgsHumedad = dto.DescuentosKgsHumedad;
                            //    cartaPorteOtroPuertos.DescuentosKgsCalidad = dto.DescuentosKgsCalidad;
                            //    cartaPorteOtroPuertos.DescuentosKgsSecada = dto.DescuentosKgsSecada;

                        }

                        if (!string.IsNullOrEmpty(dto.FotoCpBase64) && !string.IsNullOrEmpty(dto.FotoCpNombreArchivo))
                        {
                            GuardarImagen(dto, cartaPorteOtroPuertos, comando.Usuario);
                        }

                        Repositorio.GuardarCambios();
                    }

                }
            }
            catch (Exception e)
            {
                Log.Error(e,"Ocurrió un error al intentar crear la carta de porte");
                resultado.Error("", Textos.CartaPorteOtrosPuertos_Error);
            }
            return resultado;
        }

        private void GuardarImagen(CartaPorteOtrosPuertosDto dto, CartaPorteOtrosPuertos cpExistente, string usuario)
        {
            var fotoCpOtrosPuertos = Repositorio.Obtener<DocumentoExterno>(x => x.NumeroDeDocumento == dto.NumeroCartaPorte);
            var extension = Path.GetExtension(dto.FotoCpNombreArchivo);
            var fotoCp = dto.FotoCpBase64;

            if (extension.ToLower() == ".pdf")
            {
                try
                {
                    using (var document = PdfDocument.Load(new MemoryStream(Convert.FromBase64String(dto.FotoCpBase64))))
                    {
                        var dpix = ConfigurationManager.AppSettings["PdfCpeDpiX"];
                        var dpiy = ConfigurationManager.AppSettings["PdfCpeDpiY"];

                        var image = document.Render(0, string.IsNullOrEmpty(dpix) ? 600 : Convert.ToInt32(dpix), string.IsNullOrEmpty(dpiy) ? 600 : Convert.ToInt32(dpiy), PdfRenderFlags.ForPrinting | PdfRenderFlags.CorrectFromDpi);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            fotoCp = Convert.ToBase64String(ms.ToArray());
                            extension = "png";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error al convertir pdf a png de ctg {dto.NumeroCartaPorte}");
                }
            }

            var rutaArchivo = svcRepo.GuardarArchivoFotoDocumentoExterno(fotoCp, extension, dto.NumeroCartaPorte);
            if (!string.IsNullOrEmpty(rutaArchivo))
            {  
                if (fotoCpOtrosPuertos == null)
                {
                    DocumentoExternoDto fotoCartePorteDto = new DocumentoExternoDto
                    {
                        NumeroDeDocumento = dto.NumeroCartaPorte,
                        ArchivoRutaDestino = rutaArchivo,
                        ArchivoExtension = extension,

                    };

                    //cosunltar
                    var resultFotoCp = (ResultadoCrear)servicioComandos.Ejecutar(new CrearDocumentoExterno { Dto = fotoCartePorteDto, Usuario = usuario });

                    cpExistente.DocumentoExterno_Id = resultFotoCp.Id;

                }
                else
                {
                    fotoCpOtrosPuertos.ArchivoRutaDestino = rutaArchivo;
                    fotoCpOtrosPuertos.ArchivoExtension = extension;
                }
            }
            else
            {
                Log.Error($"No se pudo guardar la foto para la CP {dto.NumeroCartaPorte} con el nombre {dto.FotoCpNombreArchivo} ");
            }
        }

        private bool ValidarGrabarStock (CartaPorteOtrosPuertosDto dto) 
        {
            bool resultado = true;
            bool establecimiento = string.IsNullOrEmpty(dto?.CodigoEstablecimientoSustentable);
            bool cosecha = string.IsNullOrEmpty(dto?.ProductoCosecha);
            bool pesoNeto = !decimal.TryParse(dto?.DestinoPN, out decimal p);

            if (establecimiento || cosecha || pesoNeto || (decimal.TryParse(dto?.DestinoPN, out decimal pn) && pn == 0))
            {
                Log.Debug("No se guardo en la bd tabla RegistroStockOtrosPuertos, la cpe : {0} es sustentable pero no registra los siguientes parametros : {1}", 
                    dto?.NumeroCartaPorte, 
                    string.Join(establecimiento ? "CodigoEstablecimiento, " : string.Empty, cosecha ? "Cosecha, " : string.Empty, pesoNeto ? "PesoNeto, " : string.Empty));

                resultado = false;
            }

            return resultado;
        }

        private string ValidarFormatoCosecha(string input)
        {
            var resultado = input;
            string[] array;

            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    if (input.Contains("-"))
                    {
                        array = input.Split('-');
                        if (array.Length >= 2)
                        {
                            if (DateTime.TryParseExact(array[0], "yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFrom) && DateTime.TryParseExact(array[1], "yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTo))
                            {
                                resultado = string.Format("{0}/{1}", dateFrom.ToString("yyyy"), dateTo.ToString("yy"));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al ValidarFormatoCosecha");
            }

            return resultado;
        }
    }
}
