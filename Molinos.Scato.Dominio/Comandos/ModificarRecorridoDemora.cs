using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoDemora : Comando
    {
        public Guid InstanceId { get; set; }
        public string Motivo { get; set; }
        public bool? DemoraVehiculo { get; set; }
        public bool? DemoraEstablecimiento { get; set; }
    }
}
