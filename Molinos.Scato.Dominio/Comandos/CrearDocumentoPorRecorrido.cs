using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearDocumentoPorRecorrido : Comando
    {
        public Guid WorkflowIntanceId { get; set; }
        public string ArchivoRutaDestino { get; set; }
        public string ArchivoExtension { get; set; }
        public TipoImpresion TipoDocumentoIngreso { get; set; }
        public DateTime Fecha { get; set; }
        public byte[] Archivo { get; set; }

    }
}
