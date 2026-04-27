using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearDocumentoPorRecorridoHistorico : Comando
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
