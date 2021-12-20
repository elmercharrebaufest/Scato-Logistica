using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class RechazarDescargaUnidadItem : Comando
    {
        public IList<DescargaUnidadItemDto> Dto { get; set; }
    }
}
