using System.Collections.Generic;
using System.Drawing.Printing;
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
                [TipoImpresion.EtiquetaAuditoriaCamara] = () => new ImpEtiquetaAuditoriaCamaraDto()
            };

        }

        public ProcesadorImprimirDocumento(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresion servicioImpresion)
            : base(repositorio, conversor, log)
        {
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

            if (documento.TipoImpresion == TipoImpresion.ImpresionGenerica)
            {
                Log.Debug("Tipo de impresion: Impresion Generica");
                var formato = Repositorio.Obtener<DocumentoDeImpresionPorCentro>(x => x.DocumentoDeImpresion.Codigo == documento.Codigo && x.Centro.Id == comando.CentroId).FormatoDeImpresion;
                Log.Debug("Se intento obtener el Documento de impresion por centro");
                if (formato == null)
                {
                    Log.Debug("El Documento de impresion por centro no existe");
                    resultado.Errores.Add("", string.Format(Textos.Error_DocumentoDeImpresionNoEncontrado,documento.Codigo));
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
            }
            else
            {
                Log.Debug("Se va a enviar la impresion a la impresora {0}", impresora.Direccion);
                var firma = firmaProvider.ObtenerFirmaSinLogo();
                comando.Dto = documentoDto;
                comando.Direccion = impresora.Direccion;
                comando.TipoImpresion = documento.TipoImpresion;
                comando.Firma = firma;
                resultado = servicioImpresion.Ejecutar(comando);
                Log.Debug("Se envio exitosamente la impresion");
            }            
            return resultado;
        }

        public static void Convertir<T, Tdto>(T documento, Tdto documentoDto, IConversor conversor)
        {
            conversor.Convertir(documento, documentoDto);
        }
    }
} 