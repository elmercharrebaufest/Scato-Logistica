using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarAFIP : Comando
    {
        public int CentroId { get; set; } 
        public int TipoVehiculoId { get; set; }
        public string NumeroCartaPorte { get; set; }
    }
}
