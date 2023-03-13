using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConfigSensoresDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string SensorBarreraEntradaArriba { get; set; }
        public string SensorBarreraEntradaAbajo { get; set; }
        public string SensorPosicionIngreso { get; set; }
        public string SensorPosicionSalida { get; set; }
        public string SensorBarreraSalidaArriba { get; set; }
        public string SensorBarreraSalidaAbajo { get; set; }
        public int Centro_Id { get; set; }
    }
}
 