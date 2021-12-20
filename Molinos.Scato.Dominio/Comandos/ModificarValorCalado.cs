using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarValorCalado : Comando
    {
        public AjusteDeCalidadDto Dto { get; set; }
        public TipoDocumentoIngreso TipoDoc { get; set; }
        public string NumeroDoc { get; set; }
    }
}
