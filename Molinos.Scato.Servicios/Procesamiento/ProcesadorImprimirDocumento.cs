using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServicioImpresion;
using Ninject.Extensions.Logging;
using PrintDoc2Pdf;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorImprimirDocumento : ProcesadorComando<ImprimirDocumento>
    {
        private readonly IFirmaProvider firmaProvider;
        private readonly IServicioImpresion servicioImpresion;
        private readonly IServicioRepositorio servicioRepositorio;
        private static Dictionary<TipoImpresion, DtoExpression> dtos;
        delegate object DtoExpression();
        private void Inicializar()
        {
            dtos = new Dictionary<TipoImpresion, DtoExpression>
            {
                [TipoImpresion.AsignacionDeRuta] = () => new ImpAsignacionDeRutaDto(),
                [TipoImpresion.CertificadoDeAnalisis] = () => new ImpCertificadoDeAnalisisDto(),
                [TipoImpresion.AsigRecorrCtrolCalid] = () => new ImpAsigRecorrCtrolCalidDto(),
                [TipoImpresion.CertificadoDeCartaPorte] = () => new ImpCertificadoDeCartaPorteDto(),
                [TipoImpresion.ConstanciaDeEntregaLaser] = () => new ImpConstanciaDeEntregaLaserDto(),
                [TipoImpresion.DeclaracionFosfina] = () => new ImpDeclaracionFosfinaDto(),
                [TipoImpresion.DocumentoDeEntrada] = () => new ImpDocumentoDeEntradaDto(),
                [TipoImpresion.Formulario239] = () => new ImpFormulario239Dto(),
                [TipoImpresion.IdentificacionEnvioLoteACamara] = () => new ImpIdentificacionEnvioLoteACamaraDto(),
                [TipoImpresion.IdentificacionMicromuestra] = () => new ImpIdentificacionMicromuestraDto(),
                [TipoImpresion.IdentificacionMuestraCalado] = () => new ImpIdentificacionMuestraCaladoDto(),
                [TipoImpresion.SolicitudDeAnalisis] = () => new ImpSolicitudDeAnalisisDto(),
                [TipoImpresion.TicketPesada] = () => new ImpTicketPesadaDto(),
                [TipoImpresion.ImpresionGenerica] = () => new ImpImpresionGenericaDto(),
                [TipoImpresion.InformeDeRecepcion] = () => new ImpInformeDeRecepcionItemDto(),
                [TipoImpresion.ReciboMunicipal] = () => new ImpReciboMunicipalDto(),
                [TipoImpresion.IdentificacionMuestraAuditoria] = () => new ImpIdentificacionMuestraAuditoriaDto(),
                [TipoImpresion.EtiquetaAuditoria] = () => new ImpEtiquetaAuditoriaDto(),
                [TipoImpresion.EtiquetaIntacta] = () => new ImpEtiquetaIntactaDto(),
                [TipoImpresion.TicketPesadaBodega] = () => new ImpTicketPesadaBodegaDto(),
                [TipoImpresion.TicketPesadaAduana] = () => new ImpTicketPesadaAduanaDto(),
                [TipoImpresion.EtiquetaRubrosAnalizar] = () => new ImpEtiquetaRubrosAnalizarDto(),
                [TipoImpresion.ReciboMunicipalImportacion] = () => new ImpReciboMunicipalImportacionDto(),
                [TipoImpresion.CartaPorteUrenport] = () => new ImpCartaPorteUrenportDto(),
                [TipoImpresion.GaritaSalida] = () => new ImpGaritaSalidaDto(),
                [TipoImpresion.ResumenHojaDeRuta] = () => new ImpResumenHojaDeRutaDto(),
                [TipoImpresion.EtiquetaAuditoriaCamara] = () => new ImpEtiquetaAuditoriaCamaraDto(),
                [TipoImpresion.EtiquetaMuestraInase] = () => new ImpEtiquetaMuestraInaseDto()
            };

        }

        public ProcesadorImprimirDocumento(IServicioRepositorio servicioRepositorio, IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresion servicioImpresion)
            : base(repositorio, conversor, log)
        {
            this.servicioRepositorio = servicioRepositorio;
            this.firmaProvider = firmaProvider;
            this.servicioImpresion = servicioImpresion;
        }

        public override Resultado Ejecutar(ImprimirDocumento comando)
        {
            Inicializar();
            Log.Debug("Iniciando impresión de ImprimirDocumento en la impresora: {0}" , comando.Impresora);
            var resultado = new Resultado();
            var documento = Repositorio.ObtenerConsultaEscalar(new ObtenerImpresion(comando.Id));
            var documentoDto = dtos[documento.TipoImpresion]();
            var impresora = Repositorio.Obtener<Impresora>(x => x.Id == comando.Impresora) ?? new Impresora { Direccion = "" };
            Log.Debug("Se obtuvieron los documentos y la impresora");

            typeof(ProcesadorImprimirDocumento).GetMethod("Convertir")
                .MakeGenericMethod(documento.GetType(), documentoDto.GetType()).Invoke(null, new [] { documento, documentoDto, Conversor });
            Log.Debug("Se convirtio el archivo a su tipo original {0} {1}", documento.GetType(), documentoDto.GetType());

            switch(documento.TipoImpresion)
            {
                case TipoImpresion.ImpresionGenerica:
                    Log.Debug("Tipo de impresion: Impresion Generica");
                    var formato = Repositorio.Obtener<DocumentoDeImpresionPorCentro>(x => x.DocumentoDeImpresion.Codigo == documento.Codigo && x.Centro.Id == comando.CentroId).FormatoDeImpresion;
                    Log.Debug("Se intento obtener el Documento de impresion por centro");
                    if (formato == null)
                    {
                        Log.Debug("El Documento de impresion por centro no existe");
                        resultado.Errores.Add("", string.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, documento.Codigo));
                        return resultado;
                    }
                    Log.Debug("El Documento de impresion por centro existe");
                    var formatoDto = Conversor.Convertir<FormatoDeImpresion, FormatoDeImpresionDto>(formato);
                    Log.Debug("Se convirtio el formato de impresion");
                    Log.Debug("Se va a enviar la impresion a la impresora {0}", documento.TipoImpresion);
                    comando.Dto = documentoDto;
                    comando.Direccion = impresora.Direccion;
                    comando.Formato = formatoDto;
                    comando.TipoImpresion = documento.TipoImpresion;
                    resultado = servicioImpresion.Ejecutar(comando);

                    Log.Debug("Se envio exitosamente la impresion");
                    break;
                case TipoImpresion.EtiquetaMuestraInase:
                    Log.Debug("Se va a enviar la impresion de Inase a la impresora {0}", impresora.Direccion);
                    var firmaInase = firmaProvider.ObtenerFirmaSinLogo();
                    if (documento.WorkflowId != null)
                    {
                        comando.Dto = ObtenerDatosEtiquetaMuestraInase(documento.WorkflowId.Value);
                        comando.Direccion = impresora.Direccion;
                        comando.TipoImpresion = documento.TipoImpresion;
                        comando.Firma = firmaInase;
                        resultado = servicioImpresion.Ejecutar(comando);
                        Log.Debug("Se envio exitosamente la impresion de Inase");
                    } else
                    {
                        Log.Debug("No se envió la impresion de Inase porque no se tiene Workflow");
                    }

                    break;
                default:
                    Log.Debug("Se va a enviar la impresion a la impresora {0}", impresora.Direccion);
                    var firma = firmaProvider.ObtenerFirmaSinLogo();
                    comando.Dto = documentoDto;
                    comando.Direccion = impresora.Direccion;
                    comando.TipoImpresion = documento.TipoImpresion;
                    comando.Firma = firma;
                    resultado = servicioImpresion.Ejecutar(comando);
                    Log.Debug("Se envio exitosamente la impresion");
                    break;
            }      
            return resultado;
        }

        public static void Convertir<T, Tdto>(T documento, Tdto documentoDto, IConversor conversor)
        {
            conversor.Convertir(documento, documentoDto);
        }

        private ImpEtiquetaMuestraInaseDto ObtenerDatosEtiquetaMuestraInase(Guid workflowId)
        {
            var centroId = servicioRepositorio.ObtenerCentroIdPorInstanceId(workflowId);
            var cartaPorte = servicioRepositorio.ObtenerCartaPortePorInstanceId(workflowId);
            var numeroCartaPorte = cartaPorte.NroCartaPorte;
            var proveedor = servicioRepositorio.ObtenerProveedorPorId(cartaPorte.TitularCartaPorteId);
            var camara = servicioRepositorio.ObtenerCamaraPorMaterialPorCentro(workflowId);
            var vehiculo = servicioRepositorio.ObtenerVehiculoPorGuid(workflowId);
            if (camara != null && vehiculo != null)
            {
                var convCentro = servicioRepositorio.ObtenerConversionCentro(camara.Id, centroId);
                var codigoDeCamara = convCentro != null ? convCentro.CodigoCamara : "";
                numeroCartaPorte = camara.FormatoDeArchivo == CamaraFormatoDeArchivo.BahiaBlanca
               ? numeroCartaPorte.Substring(numeroCartaPorte.Length - 10)
               : (camara.FormatoDeArchivo == CamaraFormatoDeArchivo.Rosario ?
                codigoDeCamara.Substring(0, codigoDeCamara.Length > 3 ? 3 : codigoDeCamara.Length) :
                codigoDeCamara.Substring(0, codigoDeCamara.Length > 2 ? 2 : codigoDeCamara.Length)) +
                 vehiculo.NumeroVehiculo.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
                 numeroCartaPorte.Substring(numeroCartaPorte.Length - 10);
            }

            var dto = new ImpEtiquetaMuestraInaseDto
            {
                NumeroCartaPorte = cartaPorte.NroCartaPorte,
                WorkflowId = workflowId,
                CuitProductor = proveedor.Cuil,
                NroMuestra = numeroCartaPorte,
                Material = cartaPorte.Material,
                Patente = cartaPorte.Patente
            };
            return dto;
        }
    }
} 