using System;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    public class LiberarStockSojaEpa : Comando
    {
        public Guid InstanceId { get; set; }
    }
}
