using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ObtenerPesada : Comando
    {
        public ControlRecorridoDto Recorrido { get; set; }
    }
}
