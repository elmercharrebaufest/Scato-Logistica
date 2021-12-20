using Molinos.Scato.Dominio.Dto;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarCuposOtorgados : Comando
    {
        public string[] CentrosCodigoSap { get; set; }
        public DateTime Fecha{ get; set; }        
    }
}
