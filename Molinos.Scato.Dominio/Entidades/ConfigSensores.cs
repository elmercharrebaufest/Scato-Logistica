using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConfigSensores : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string SensorBarreraEntradaArriba { get; set; }
        public virtual string SensorBarreraEntradaAbajo { get; set; }
        public virtual string SensorPosicionIngreso { get; set; }
        public virtual string SensorPosicionSalida { get; set; }
        public virtual string SensorBarreraSalidaArriba { get; set; }
        public virtual string SensorBarreraSalidaAbajo { get; set; }
        public virtual Centro Centro { get; set; }

    }
}
