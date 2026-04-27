using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearDocumentoPorRecorridoHistorico
        : ProcesadorComando<CrearDocumentoPorRecorridoHistorico>
    {
        private const string PDF_EXTENSION = ".pdf";
        private const string FOTOS_PATH_CONFIG_KEY = "FotosPath";
        private const string ERROR_KEY_DATOS_RECORRIDO = "DatosRecorrido";
        private const string ERROR_KEY_PREFIX_GUARDAR_DOCUMENTO = "GuardarDocumento-";
        private const string ERROR_KEY_GENERAL = "Error";
        private const string DATE_FORMAT = "yyyyMMdd";

        private readonly IConfiguracionProvider _configuracion;

        // 🔒 Límite de concurrencia para escritura de archivos
        private static readonly SemaphoreSlim _fileSemaphore = new SemaphoreSlim(4);

        public ProcesadorCrearDocumentoPorRecorridoHistorico(
            IRepositorio repositorio,
            IConversor conversor,
            ILogger log,
            IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            _configuracion = configuracion;
        }

        public override Resultado Ejecutar(CrearDocumentoPorRecorridoHistorico comando)
        {
            // Llama a la versión asíncrona y espera el resultado de forma síncrona
            // para cumplir con la firma requerida por la clase base.
            return EjecutarAsync(comando).GetAwaiter().GetResult();
        }

        // Método asíncrono original renombrado para uso interno
        private async Task<Resultado> EjecutarAsync(CrearDocumentoPorRecorridoHistorico comando)
        {
            Log.Info("Ejecutando procesamiento CrearDocumentoPorRecorridoHistorico");
            var resultado = new Resultado();

            try
            {
                var tipoImpresion = TipoImpresion.CartaDePorteElectronica;
                var datosRecorrido = Repositorio.ObtenerDatosRecorridoRelacionadosConCartaPorteElectronica(comando.FechaInicio, comando.FechaFin);

                if (datosRecorrido == null || !datosRecorrido.Any())
                {
                    resultado.Errores.Add(ERROR_KEY_DATOS_RECORRIDO,
                        "No se encontraron datos de recorrido asociados.");
                    return resultado;
                }

                var tareas = datosRecorrido.Select(dato => Task.Run(async () =>
                {
                    if (dato.Pdf == null || dato.Pdf.Length == 0)
                    {
                        resultado.Errores.Add(
                            $"{ERROR_KEY_PREFIX_GUARDAR_DOCUMENTO}{dato.Id}",
                            $"PDF vacío o nulo para el RecorridoId: {dato.Id}");
                        return;
                    }

                    await _fileSemaphore.WaitAsync();
                    try
                    {
                        DateTime fechaCacheado =
                            dato.FechaCacheado.HasValue && dato.FechaCacheado.Value != DateTime.MinValue
                                ? dato.FechaCacheado.Value
                                : DateTime.Now;

                        var guardado = await GuardarDocumentoEnServidorAsync(
                            dato.Pdf,
                            dato.InstanciaWorkflow,
                            fechaCacheado);

                        if (!guardado.Exitoso)
                        {
                            resultado.Errores.Add(
                                $"{ERROR_KEY_PREFIX_GUARDAR_DOCUMENTO}{dato.Id}",
                                guardado.Error);
                            return;
                        }

                        lock (Repositorio)
                        {
                            Repositorio.Agregar(new DocumentoPorRecorrido
                            {
                                RecorridoId = dato.Id,
                                Path = guardado.RutaDocumento,
                                Extension = PDF_EXTENSION,
                                Tipo = tipoImpresion,
                                FechaDeGuardado = fechaCacheado
                            });
                        }
                    }
                    finally
                    {
                        _fileSemaphore.Release();
                    }
                }));

                await Task.WhenAll(tareas);
                Repositorio.GuardarCambios();

                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error("Error general en CrearDocumentoPorRecorridoHistorico", ex);
                resultado.Errores.Add(ERROR_KEY_GENERAL, ex.Message);
                return resultado;
            }
        }

        private Task<GuardarDocumentoResultado> GuardarDocumentoEnServidorAsync(
            byte[] pdf,
            Guid workflowInstanceId,
            DateTime fecha)
        {
            return Task.Run(() =>
            {
                try
                {
                    var basePath = _configuracion.AppSettings[FOTOS_PATH_CONFIG_KEY];
                    if (string.IsNullOrWhiteSpace(basePath))
                    {
                        return new GuardarDocumentoResultado
                        {
                            Exitoso = false,
                            Error = $"Configuración '{FOTOS_PATH_CONFIG_KEY}' no encontrada o vacía"
                        };
                    }

                    var tipoDocPath = TipoImpresion.CartaDePorteElectronica.ToString();
                    var fechaPath = fecha.ToString(DATE_FORMAT);
                    var nombreFile = workflowInstanceId.ToString();

                    var rutaDestino = Path.Combine(
                        basePath,
                        tipoDocPath,
                        fechaPath,
                        nombreFile + PDF_EXTENSION
                    );

                    Directory.CreateDirectory(Path.GetDirectoryName(rutaDestino));
                    File.WriteAllBytes(rutaDestino, pdf);

                    return new GuardarDocumentoResultado
                    {
                        Exitoso = true,
                        RutaDocumento = rutaDestino
                    };
                }
                catch (Exception ex)
                {
                    Log.Error("Error al guardar documento en servidor", ex);
                    return new GuardarDocumentoResultado
                    {
                        Exitoso = false,
                        Error = ex.Message
                    };
                }
            });
        }

        private class GuardarDocumentoResultado
        {
            public bool Exitoso { get; set; }
            public string RutaDocumento { get; set; }
            public string Error { get; set; }
        }
    }
}