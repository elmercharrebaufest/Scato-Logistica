using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirInformeDeRecepcion : Comando
    {
        public List<ImpInformeDeRecepcionDto> Dto { get; set; }
        public string Impresora { get; set; }
        public FirmaDto Firma { get; set; }
    }
}
