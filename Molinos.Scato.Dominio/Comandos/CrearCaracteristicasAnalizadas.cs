using System;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearCaracteristicasAnalizadas : Comando
    {
        public int IdRecorridoIngreso { get; set; }
        public int IdRecorridoEgreso { get; set; }
    }
}
