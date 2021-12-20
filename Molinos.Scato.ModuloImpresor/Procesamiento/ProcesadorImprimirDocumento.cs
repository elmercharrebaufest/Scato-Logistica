using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Molinos.Scato.ModuloImpresor.Zebra;
using Ninject.Extensions.Logging;
using PrintDoc2Pdf;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirDocumento : ProcesadorComando<ImprimirDocumento>
    {

        private static Dictionary<TipoImpresion, DocumentoImpresion> impresoras;
        private static Dictionary<TipoImpresion, DtoExpression> dtos;
        delegate object DtoExpression();
        private void Inicializar()
        {
            impresoras = new Dictionary<TipoImpresion, DocumentoImpresion>
            {
                [TipoImpresion.AsignacionDeRuta] = new AsignacionDeRuta(),
                [TipoImpresion.CertificadoDeAnalisis] = new CertificadoDeAnalisis(),
                [TipoImpresion.AsigRecorrCtrolCalid] = new AsigRecorrCtrolCalid(),
                [TipoImpresion.CertificadoDeCartaPorte] = new CertificadoDeCartaPorte(),
                [TipoImpresion.ConstanciaDeEntregaLaser] = new ConstanciaDeEntregaLaser(),
                [TipoImpresion.DeclaracionFosfina] = new DeclaracionFosfina(),
                [TipoImpresion.DocumentoDeEntrada] = new DocumentoDeEntrada(),
                [TipoImpresion.Formulario239] = new Formulario239(),
                [TipoImpresion.IdentificacionEnvioLoteACamara] = new IdentificacionEnvioLoteACamara(),
                [TipoImpresion.IdentificacionMicromuestra] = new IdentificacionMicromuestra(),
                [TipoImpresion.IdentificacionMuestraCalado] = new IdentificacionMuestraCalado(),
                [TipoImpresion.SolicitudDeAnalisis] = new SolicitudDeAnalisis(),
                [TipoImpresion.TicketPesada] = new TicketPesada(),
                [TipoImpresion.ImpresionGenerica] = new ImpresionGenerica(),
                [TipoImpresion.InformeDeRecepcion] = new InformeDeRecepcion(),
                [TipoImpresion.ReciboMunicipal] = new ReciboMunicipalEtiqueta(),
                [TipoImpresion.IdentificacionMuestraAuditoria] = new IdentificacionMuestraAuditoria(),
                [TipoImpresion.EtiquetaAuditoria] = new EtiquetaAuditoria(),
                [TipoImpresion.EtiquetaIntacta] = new EtiquetaIntacta(),
                [TipoImpresion.TicketPesadaBodega] = new TicketPesadaBodega(),
                [TipoImpresion.TicketPesadaAduana] = new TicketPesadaAduana(),
                [TipoImpresion.EtiquetaRubrosAnalizar] = new EtiquetaRubrosAnalizar(),
                [TipoImpresion.ReciboMunicipalImportacion] = new ReciboMunicipalImportacionEtiqueta(),
                [TipoImpresion.CartaPorteUrenport] = new CartaPorteUrenport(),
                [TipoImpresion.GaritaSalida] = new GaritaSalidaEtiqueta(),
                [TipoImpresion.ResumenHojaDeRuta] = new ResumenHojaDeRuta(),
                [TipoImpresion.EtiquetaAuditoriaCamara] = new EtiquetaAuditoriaCamara()
            };
        }

        public ProcesadorImprimirDocumento(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirDocumento comando)
        {
            Inicializar();
            var resultado = new Resultado();
            if (comando.TipoImpresion == TipoImpresion.ImpresionGenerica)
            {
                for (var i = 0; i < comando.CantCopias; i++)
                {
                    impresoras[comando.TipoImpresion].Imprimir(comando.Dto, comando.Direccion, comando.Formato);
                }
                Log.Debug("Se envio exitosamente la impresion");
            }
            else
            {
                Log.Debug("Se va a enviar la impresion a la impresora {0}", comando.Direccion);

                for (int i = 0; i < comando.CantCopias; i++)
                {
                    impresoras[comando.TipoImpresion].Imprimir(comando.Dto, comando.Direccion, comando.Firma);    
                }
                Log.Debug("Se envio exitosamente la impresion");
            }

            if (comando.Direccion == "")
            {
                try
                {
                    var resultadoPdf = new ResultadoPrevisualizar();
                    resultado = resultadoPdf;
                    var printDocument = impresoras[comando.TipoImpresion];
                    var printer = new pdfPrinter();

                    if (!string.IsNullOrEmpty(printDocument.ZplCode))
                    {
                        Log.Debug($"Se va a renderizar contra la impresora {ConfigurationManager.AppSettings["ZebraPrinterIp"]}, el ticket: {printDocument.ZplCode}");
                        var imagen = ZebraPrinter.ObtenerImagen(printDocument.ZplCode, ConfigurationManager.AppSettings["ZebraPrinterIp"], Log);
                            printer.Document = new ImpresorDeImagenes(imagen);
                    }
                    else
                    {
                        printer.Document = printDocument;
                    }
                    printer.Print();
                    resultadoPdf.Archivo = printer.File;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error al generar etiqueta");
                }
            }
            
            return resultado;
        }
    }
} 