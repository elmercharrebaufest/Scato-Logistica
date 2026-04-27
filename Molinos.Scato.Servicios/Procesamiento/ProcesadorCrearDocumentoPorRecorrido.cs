using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.IO;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearDocumentoPorRecorrido : ProcesadorCrear<CrearDocumentoPorRecorrido, DocumentoPorRecorrido>
    {
        protected readonly IServicioRepositorio servicio;
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorCrearDocumentoPorRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioRepositorio servicio, IConfiguracionProvider configuracion) : base(repositorio, conversor, log)
        {
            this.servicio = servicio;
            this.configuracion = configuracion;
        }

        protected override DocumentoPorRecorrido CrearEntidad(CrearDocumentoPorRecorrido comando)
        {
            GuardarDocumentoEnServidor(comando);
           
            return new DocumentoPorRecorrido
            {
                RecorridoId = servicio.ObtenerRecorridoIdPorGuid(comando.WorkflowIntanceId),
                Path = comando.ArchivoRutaDestino,
                Extension = comando.ArchivoExtension,
                Tipo = comando.TipoDocumentoIngreso,
                FechaDeGuardado = comando.Fecha
            };
        }

        protected override void Validar(CrearDocumentoPorRecorrido comando, Resultado resultado)
        {
            if (comando.WorkflowIntanceId == Guid.Empty)
            {
                resultado.Error("", "WorkflowInvalido");
            }
            if (string.IsNullOrEmpty(comando.ArchivoExtension))
            {
                resultado.Error("", "Extension no existe");
            }
            if (!Repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowIntanceId))
            {
                resultado.Error("", "No existe recorrido asociado al workflow");
            }
            if (Repositorio.Existe<DocumentoPorRecorrido>(d => d.Recorrido.InstanciaWorkflow == comando.WorkflowIntanceId && d.Tipo == comando.TipoDocumentoIngreso))
            {
                resultado.Error("", "Ya existe un documento de este tipo para el recorrido");
            }
        }

        private void GuardarDocumentoEnServidor(CrearDocumentoPorRecorrido comando)
        {
            try
            {
                if(comando.Archivo == null || comando.Archivo.Length == 0)
                {
                    var nroCtg = Repositorio.ObtenerProyeccion<Recorrido, string>(r => r.InstanciaWorkflow == comando.WorkflowIntanceId, p => p.Vehiculo.CartaPorte.NroCartaPorte);
                    if (string.IsNullOrEmpty(nroCtg) || !long.TryParse(nroCtg, out long nroCtgLong))
                    {
                        Log.Error("No se encontró Carta de Porte asociada al Recorrido");
                        throw new Exception("No se encontró Carta de Porte asociada al Recorrido");
                    }
                    var carta = Repositorio.Obtener<CartaPorteElectronica>(c => c.NroCTG == nroCtgLong);
                    if (carta == null || carta.Pdf == null || carta.Pdf.Length == 0)
                    {
                        Log.Error("No se encontró PDF de Carta de Porte asociada al Recorrido");
                        throw new Exception("No se encontró PDF de Carta de Porte asociada al Recorrido");
                    }
                    comando.Archivo = carta.Pdf;
                }
                
                var basePath = configuracion.AppSettings["FotosPath"];

                // Subcarpeta por tipo de documento
                var tipoDocPath = comando.TipoDocumentoIngreso.ToString();

                // Subcarpeta por fecha (yyyyMMdd)
                var fechaPath = comando.Fecha.ToString("yyyyMMdd");

                // Extensión (por defecto .pdf)
                var extension = string.IsNullOrWhiteSpace(comando.ArchivoExtension)
                    ? ".pdf"
                    : (comando.ArchivoExtension.StartsWith(".")
                        ? comando.ArchivoExtension
                        : "." + comando.ArchivoExtension);

                // Nombre del archivo → WorkflowIntanceId
                string nombreFile = comando.WorkflowIntanceId.ToString();

                // Ruta completa
                var rutaDestino = Path.Combine(basePath, tipoDocPath, fechaPath, nombreFile + extension);

                // Crear directorio si no existe
                var directorio = Path.GetDirectoryName(rutaDestino);
                if (!Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                // Guardar el archivo
                File.WriteAllBytes(rutaDestino, comando.Archivo);

                // Devolver la ruta resultante en el comando
                comando.ArchivoRutaDestino = rutaDestino;

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Guardar documento en servidor", ex);
            }
        }
    }
}