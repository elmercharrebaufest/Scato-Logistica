using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DocumentoDeOrigenDto
    {
        public string ActividadTitulo { get; set; }
        public string Actividad { get; set; }
        public Guid InstanceId { get; set; }
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public int RecorridoId { get; set; }
        public bool EsActividad { get; set; }
    }
}
