using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models.ArchivosTxt;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.GenerarArchivosOncca)]
    public class GenerarArchivosOnccaController : BaseController
    {

        private readonly IServicioComandos servicioComandos;
        private readonly ILogger log;

        public GenerarArchivosOnccaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
        }

        public ActionResult Index()
        {
            SetearVista();
            var centros = servicio.ListarCentros();
            return View(centros);
        }

        [HttpPost]
        public ActionResult Generar(IEnumerable<int> centrosSeleccionados, string tipoArchivo, DateTime? fechaInicio,
                                    DateTime? fechaFin)
        {
            if (centrosSeleccionados == null || !centrosSeleccionados.Any())
            {
                ModelState.AddModelError("", Textos.SeleccionarCentro_Error);
                SetearVista();
                return View("Index", servicio.ListarCentros());
            }
            if (!fechaFin.HasValue)
            {
                ModelState.AddModelError("fechaFin", String.Format(Textos.Error_Requerido,Textos.FechaFin));
                SetearVista();
                return View("Index", servicio.ListarCentros());
            }
            if (!fechaInicio.HasValue)
            {
                ModelState.AddModelError("fechaInicio", String.Format(Textos.Error_Requerido, Textos.FechaInicio));
                SetearVista();
                return View("Index", servicio.ListarCentros());
            }
            var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                #region contenido
                var contenido = new StringBuilder();
                log.Info("Se comienza la generación de archivos para la Oncca. Tipo de archivo: {0}", tipoArchivo);
                
                var tipoDeWorkflow = tipoArchivo == "MCPE" ? TipoDeWorkflow.Egreso : TipoDeWorkflow.Ingreso;
                var registros = servicio.ListarArchivoOncca(centrosSeleccionados.ToList(), fechaInicio.Value, fechaFin.Value, tipoDeWorkflow);

                if (tipoDeWorkflow == TipoDeWorkflow.Egreso)
                {
                    GenerarOnccaEmitidas(registros, contenido);
                }
                else
                {
                    GenerarOnccaRecibidas(registros, contenido);
                }


                #endregion

                if (contenido.Length == 0)
                {
                    ModelState.AddModelError("", "No existen datos para el/los centro/s seleccionados");
                    SetearVista();
                    return View("Index", servicio.ListarCentros());
                }

                streamWriter.Write(contenido);
                streamWriter.Flush();
                var fileStream = new MemoryStream(memoryStream.ToArray());
                fileStream.Seek(0, SeekOrigin.Begin);
                return File(fileStream, "application/octet-stream", tipoArchivo + ".txt");
            }
        }

        private static void GenerarOnccaRecibidas(IEnumerable<OnccaEmitidasDto> registros, StringBuilder contenido)
        {
            foreach (var registro in registros)
            {
                contenido.AppendLine(
                    TxtHelper.GetTxtDataRow(
                        new OnccaRecibidas
                            {
                                TipoDeTransporte = registro.TipoDeTransporte,
                                TipoDeCartaDePorte = registro.TipoDeCartaDePorte,
                                NroCartaDePorte = registro.NroCartaDePorte,
                                NumeroDeCee = registro.NumeroDeCee,
                                NumeroDeCTG = registro.NumeroDeCTG,
                                FechaDeCarga = registro.FechaDeCarga,
                                CuitTitularCartaDePorte = registro.CuitTitularCartaDePorte,
                                CuitIntermediario = registro.CuitIntermediario,
                                CuitDelRemitenteComercial = registro.CuitDelRemitenteComercial,
                                CuitCorredor = registro.CuitCorredor,
                                CuitRepresentanteEntregador = registro.CuitRepresentanteEntregador,
                                CuitDestinario = registro.CuitDestinario,
                                CuitEstablecimientoDestino = registro.CuitEstablecimientoDestino,
                                CuitTransportista = registro.CuitTransportista,
                                CuilDelChofer = registro.CuilDelChofer,
                                Cosecha = registro.Cosecha,
                                CodigoDeEspecie = registro.CodigoDeEspecie,
                                TipoDeGrano = registro.TipoDeGrano,
                                Contrato = registro.Contrato,
                                TipoDePesado = registro.TipoDePesado,
                                PesoNetoDeCarga = registro.PesoNetoDeCarga,
                                CodigoDeEstablecimientoDeProcedencia = registro.CodigoDeEstablecimientoDeProcedencia,
                                CodigoDeLocalidadDeProcedencia = registro.CodigoDeLocalidadDeProcedencia,
                                CodigoDeEstablecimientoDestino = registro.CodigoDeEstablecimientoDestino,
                                CodigoDeLocalidadDeDestino = registro.CodigoDeLocalidadDeDestino,
                                KmARecorrer = registro.KmARecorrer,
                                Patente = registro.Patente,
                                AcopladoPatente = registro.AcopladoPatente,
                                TarifaPorTonelada = registro.TarifaPorTonelada,
                                FechaDeDescarga = registro.FechaDeDescarga,
                                FechaDeArriboADestino = registro.FechaDeArriboADestino,
                                PesoNetoDeDescarga = registro.PesoNetoDeDescarga,
                                CuitEstablecimientoRedestino = registro.CuitEstablecimientoRedestino,
                                FleteTarifaDeReferencia = registro.FleteTarifaDeReferencia
                            }, typeof (OnccaRecibidas).GetProperties()));
            }
        }

        private static void GenerarOnccaEmitidas(IEnumerable<OnccaEmitidasDto> registros, StringBuilder contenido)
        {
            foreach (var registro in registros)
            {
                contenido.AppendLine(
                    TxtHelper.GetTxtDataRow(
                        new OnccaEmitidas
                            {
                                TipoDeTransporte = registro.TipoDeTransporte,
                                TipoDeCartaDePorte = registro.TipoDeCartaDePorte,
                                NroCartaDePorte = registro.NroCartaDePorte,
                                NumeroDeCee = registro.NumeroDeCee,
                                NumeroDeCTG = registro.NumeroDeCTG,
                                FechaDeCarga = registro.FechaDeCarga,
                                CuitTitularCartaDePorte = registro.CuitTitularCartaDePorte,
                                CuitIntermediario = registro.CuitIntermediario,
                                CuitDelRemitenteComercial = registro.CuitDelRemitenteComercial,
                                CuitCorredor = registro.CuitCorredor,
                                CuitRepresentanteEntregador = registro.CuitRepresentanteEntregador,
                                CuitDestinario = registro.CuitDestinario,
                                CuitEstablecimientoDestino = registro.CuitEstablecimientoDestino,
                                CuitTransportista = registro.CuitTransportista,
                                CuilDelChofer = registro.CuilDelChofer,
                                Cosecha = registro.Cosecha,
                                CodigoDeEspecie = registro.CodigoDeEspecie,
                                TipoDeGrano = registro.TipoDeGrano,
                                Contrato = registro.Contrato,
                                TipoDePesado = registro.TipoDePesado,
                                PesoNetoDeCarga = registro.PesoNetoDeCarga,
                                CodigoDeEstablecimientoDeProcedencia = registro.CodigoDeEstablecimientoDeProcedencia,
                                CodigoDeLocalidadDeProcedencia = registro.CodigoDeLocalidadDeProcedencia,
                                CodigoDeEstablecimientoDestino = registro.CodigoDeEstablecimientoDestino,
                                CodigoDeLocalidadDeDestino = registro.CodigoDeLocalidadDeDestino,
                                KmARecorrer = registro.KmARecorrer,
                                Patente = registro.Patente,
                                AcopladoPatente = registro.AcopladoPatente,
                                TarifaPorTonelada = registro.TarifaPorTonelada,
                                FleteTarifaDeReferencia = registro.FleteTarifaDeReferencia
                            }, typeof (OnccaEmitidas).GetProperties()));
            }
        }

        private void SetearVista()
        {
            var tiposArchivo = new Dictionary<string, string>
                {
                    {"MCPR", Textos.GenerarArchivosOncca_TipoDeArchivo_Recibidas},
                    {"MCPE", Textos.GenerarArchivosOncca_TipoDeArchivo_Emitidas}
                };
            ViewBag.TiposArchivo = tiposArchivo.ToSelectList(x => x.Key, x => x.Value);
        }
    }
}
