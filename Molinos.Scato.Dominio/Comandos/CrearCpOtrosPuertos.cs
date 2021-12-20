using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCpOtrosPuertos : Comando
    {
        public List<CartaPorteOtrosPuertosDto> Dto { get; set; }
    }
}
