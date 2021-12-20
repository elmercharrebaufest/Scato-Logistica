using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirEtiquetaAuditoria : Comando
    {
        public ImpEtiquetaAuditoriaDto Dto { get; set; }
        public FirmaDto Firma { get; set; }
    }
}
