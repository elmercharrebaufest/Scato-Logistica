using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class RegistrarMarcaDeTiempo : Comando
    {
        public TipoRegistroMarcaDeTiempo Tipo { get; set; }
        public string CodigoDispositivo { get; set; }
        public int? PuestoDeTrabajoId { get; set; }
        public string NumeroDeTarjeta { get; set; }
        public string Patente { get; set; }
        public TipoIdentificacionPorPuesto? Trigger { get; set; }
        public Guid? InstanceId { get; set; }
    }

    public enum TipoRegistroMarcaDeTiempo
    {
        Inicio,
        Identificacion,
        Fin,
        InicioOFinPorSensor
    }
}
