using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    public class ResultadoConsultarCategoriaVehiculo : Resultado
    {
        public Dictionary<DateTime, decimal> ImporteTasaMunicipal { get; set; }
        public bool EsAutomotor { get; set; } = true;
    }
}
