using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarFirma : Comando
    {
        public FirmaDto Dto { get; set; }
    }
}
