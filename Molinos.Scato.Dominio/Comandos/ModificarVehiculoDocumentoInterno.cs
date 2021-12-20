using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarVehiculoDocumentoInterno : Comando
    {
        public Guid InstanceId { get; set; }
        public string DocumentoInternoSap { get; set; }
        public string NumeroDeDocumentoSap { get; set; }
    }
}
